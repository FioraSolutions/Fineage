import re
def read_anim(path):
    """Return {section:{path:[(time,(vals))]}} for m_RotationCurves/m_PositionCurves/m_ScaleCurves"""
    txt=open(path).read().split('\n')
    res={}; sec=None; cur=[]; t=None
    for line in txt:
        m=re.match(r'^  (m_\w+):',line)
        if m: sec=m.group(1); cur=[]; continue
        if sec not in ('m_RotationCurves','m_PositionCurves','m_ScaleCurves'): continue
        s=line.strip()
        if s.startswith('time:'): t=float(s.split(':')[1])
        elif s.startswith('value:'):
            nums=[float(x) for x in re.findall(r'[xyzw]: ([-\deE.+]+)',s)]
            cur.append((t,tuple(nums)))
        elif s.startswith('path:'):
            res.setdefault(sec,{})[s[5:].strip()]=cur; cur=[]
    return res
