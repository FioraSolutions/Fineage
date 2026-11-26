#!/usr/bin/env bash
# Python guards shared by build-and-test (push) and pr-sanity (PR).
# Keep in sync: any new push-only check belongs here so PRs catch drift before merge.
set -euo pipefail

repo_root="$(cd "$(dirname "$0")/.." && pwd)"
cd "$repo_root"

python3 scripts/check-anim-rotation-hygiene.py
python3 scripts/check-frozen-incoming-packet-actions.py
python3 scripts/check-frozen-net-domain-maps.py
python3 scripts/check-net-zone-inbound-opcodes.py

python3 scripts/check-release-secrets.py --self-test
python3 scripts/bake-playtest-endpoint.py --self-test
python3 scripts/release-notes.py --self-test
python3 scripts/pack-client.py --self-test

python3 scripts/check-client-asmdef-layout.py
python3 scripts/check-deco-archive-fixture.py
python3 scripts/check-unity-meta-guids.py
python3 scripts/check-interlude-window-layout.py
python3 scripts/check-unity-editmode-guards.py
bash scripts/check-interlude-javac.sh
python3 tools/interlude-extract/lobby_mesh_resolve.py --check
python3 tools/interlude-extract/check_lobby_camera_zone.py
unknown_arg_err="$(mktemp)"
set +e
bash scripts/login-lobby-rebuild.sh --not-a-real-flag >"$unknown_arg_err" 2>&1
unknown_arg_status=$?
set -e
if [[ "$unknown_arg_status" -ne 2 ]] || ! grep -q "unknown argument '--not-a-real-flag'" "$unknown_arg_err"; then
  echo "ci-client-guards: login-lobby-rebuild.sh unknown argument exited $unknown_arg_status" >&2
  cat "$unknown_arg_err" >&2
  rm -f "$unknown_arg_err"
  exit 1
fi
rm -f "$unknown_arg_err"

python3 scripts/test_asset_import_policy_meta.py
python3 scripts/check-asset-import-policy.py

python3 scripts/check-npc-monster-meter-contract.py
python3 scripts/check-weapon-mesh-topology.py
python3 scripts/check-elven-bsp-csg-fixtures.py
python3 scripts/audit-actor-alpha-materials.py
python3 scripts/check-paperdoll-armor-shader.py

backend_contract="${FINEAGE_BACKEND_CONTRACT:-$GITHUB_WORKSPACE/Fineage-Backend/src/Fineage.Net.Contract}"
if [[ ! -d "$backend_contract" ]]; then
  echo "ci-client-guards: Fineage-Backend Net contract missing at $backend_contract" >&2
  exit 1
fi

python3 scripts/test_net_contract_unity_cs9.py

FINEAGE_REQUIRE_CONTRACT_SYNC=1 python3 scripts/sync-net-contract.py --check \
  --backend "$backend_contract" \
  --ref ''

python3 scripts/check-net-contract-unity-compile.py