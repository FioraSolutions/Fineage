#!/usr/bin/env bash
# Ensure a public-login smoke account+char, then drive Editor worldprobe (LobbyClean).
set -euo pipefail

REQUEST="/tmp/fineage-dev-rp-play.request"
RESULT="/tmp/fineage-rp-probe.result"
TIMEOUT_SEC="${FINEAGE_RP_TIMEOUT:-480}"

mapfile -t INFO < <(python3 - <<'PY'
import os, socket, struct, uuid
from pathlib import Path

host = os.environ.get("FINEAGE_LOGIN_HOST", "login.fineage.evpoint.by")
port = int(os.environ.get("FINEAGE_LOGIN_PORT", "2106"))
creds_path = Path("/tmp/fineage-smoke-creds.txt")
char_path = Path("/tmp/fineage-smoke-char.txt")

def mp_str(s: str) -> bytes:
    b = s.encode(); n = len(b)
    if n < 32:
        return bytes([0xa0 | n]) + b
    if n < 256:
        return bytes([0xd9, n]) + b
    return bytes([0xda]) + struct.pack(">H", n) + b

def mp_int(i: int) -> bytes:
    if 0 <= i <= 127:
        return bytes([i])
    if 0 <= i <= 255:
        return bytes([0xcc, i])
    return bytes([0xd2]) + struct.pack(">i", i)

def mp_array(items):
    return bytes([0x90 | len(items)]) + b"".join(items)

def frame(t, body):
    return struct.pack("<HH", 4 + len(body), t) + body

def read_frame(sock):
    hdr = sock.recv(4)
    if len(hdr) < 4:
        return None
    total, typ = struct.unpack("<HH", hdr)
    body = b""
    need = total - 4
    while len(body) < need:
        chunk = sock.recv(need - len(body))
        if not chunk:
            break
        body += chunk
    return typ, body

def connect_auth(account, password):
    sock = socket.create_connection((host, port), timeout=15)
    sock.settimeout(15)
    sock.sendall(frame(1, mp_array([mp_int(1), mp_str("unity")])))
    if read_frame(sock) is None:
        sock.close()
        return None
    sock.sendall(frame(2, mp_array([mp_str(account), mp_str(password)])))
    auth = read_frame(sock)
    if auth is None or auth[0] != 1024:
        sock.close()
        return None
    return sock

env_user = os.environ.get("FINEAGE_RP_USER") or ""
env_pass = os.environ.get("FINEAGE_RP_PASS") or ""
env_char = os.environ.get("FINEAGE_RP_CHAR") or ""

account = env_user
password = env_pass
if account and password:
    sock = connect_auth(account, password)
    if sock is None:
        raise SystemExit("auth failed for FINEAGE_RP_USER")
    sock.close()
else:
    account = password = None
    if creds_path.exists():
        lines = creds_path.read_text().strip().splitlines()
        if len(lines) >= 2:
            sock = connect_auth(lines[0], lines[1])
            if sock is not None:
                account, password = lines[0], lines[1]
                sock.close()
    if not account:
        account = "smk" + uuid.uuid4().hex[:10]
        password = "pw" + uuid.uuid4().hex[:10]
        sock = socket.create_connection((host, port), timeout=15)
        sock.settimeout(15)
        sock.sendall(frame(1, mp_array([mp_int(1), mp_str("unity")])))
        read_frame(sock)
        sock.sendall(frame(20, mp_array([mp_str(account), mp_str(password)])))
        reg = read_frame(sock)
        sock.close()
        if reg is None or reg[0] != 1020:
            raise SystemExit("register failed: " + repr(reg))
        creds_path.write_text(account + "\n" + password + "\n")

char_name = env_char or (char_path.read_text().strip() if char_path.exists() else "")
sock = connect_auth(account, password)
if sock is None:
    raise SystemExit("auth failed")
sock.sendall(frame(22, b"\x90"))
sock.settimeout(2)
has_roster = False
try:
    while True:
        fr = read_frame(sock)
        if not fr:
            break
        if fr[0] == 1022 and len(fr[1]) > 2:
            has_roster = True
except Exception:
    pass
sock.settimeout(15)
if not has_roster or not char_name:
    char_name = char_name or ("smk" + account[-4:])
    body = mp_array(
        [mp_str(char_name), mp_int(0), mp_int(0), mp_int(0), mp_int(0), mp_int(0), mp_int(0)]
    )
    sock.sendall(frame(21, body))
    while True:
        fr = read_frame(sock)
        if not fr:
            raise SystemExit("create no reply")
        if fr[0] in (1021, 1099, 1022):
            break
sock.close()
char_path.write_text(char_name + "\n")
print(account)
print(password)
print(char_name)
PY
)

USER_NAME="${INFO[0]}"
PASS="${INFO[1]}"
CHAR="${INFO[2]}"

rm -f "$RESULT"
printf 'worldprobe %s %s %s\n' "$USER_NAME" "$PASS" "$CHAR" > "$REQUEST"
echo "[smoke-lobby-clean] worldprobe $USER_NAME *** $CHAR (Unity Editor must be open on Fineage)"

deadline=$((SECONDS + TIMEOUT_SEC))
while (( SECONDS < deadline )); do
  if [[ -f "$RESULT" ]]; then
    echo "[smoke-lobby-clean] result:"
    cat "$RESULT"
    if grep -q '^ok ' "$RESULT"; then
      exit 0
    fi
    exit 2
  fi
  sleep 2
done

echo "[smoke-lobby-clean] timeout after ${TIMEOUT_SEC}s waiting for $RESULT" >&2
exit 3
