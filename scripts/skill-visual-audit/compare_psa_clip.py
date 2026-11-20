import sys,math
from animyaml import *; from psa import *
def interp(curve,t):
    # linear interp between keys (approx for hermite)
    for i in range(len(curve)-1):
        t0,v0=curve[i];t1,v1=curve[i+1]
        if t0<=t<=t1:
            a=(t-t0)/(t1-t0) if t1>t0 else 0
            return tuple(v0[j]+(v1[j]-v0[j])*a for j in range(len(v0)))
    return curve[-1][1]
def conv(i,pos,q,S):
    x,y,z,w=q
    rq=(-x,y,z,-w) if i==0 else (-x,y,z,w)
    return (-pos[0]*S,pos[1]*S,pos[2]*S),rq
psaf,take,anim=sys.argv[1:4]
b,a,k=read_psa(psaf); s=[x for x in a if x['name']==take][0]; kk=k(s)
r=read_anim(anim); rot=r['m_RotationCurves']; pc=r['m_PositionCurves']
leaf={p.split('/')[-1].lower():p for p in rot}
num=den=0
for fr in range(s['num']):
  for i,bn in enumerate(b):
    p=leaf.get(bn['name'].lower())
    if p and p in pc:
      u=interp(pc[p],fr/24.0); P=kk[fr][i][0]; src=(-P[0],P[1],P[2])
      for j in range(3): num+=u[j]*src[j]; den+=src[j]*src[j]
S=num/den; print('S',S, 'nbones',len(b),'mapped',sum(1 for bn in b if bn['name'].lower() in leaf))
maxr=maxp=0;worst=None
for fr in range(0,s['num']):
  for i,bn in enumerate(b):
    p=leaf.get(bn['name'].lower())
    if not p: continue
    P,Q=conv(i,*kk[fr][i],S)
    u=interp(rot[p],fr/24.0)
    d=abs(sum(Q[j]*u[j] for j in range(4)))
    err=1-min(1,d)
    if err>maxr: maxr=err;worst=(bn['name'],fr,Q,u)
    if p in pc:
      up=interp(pc[p],fr/24.0); e=max(abs(P[j]-up[j]) for j in range(3)); maxp=max(maxp,e)
print('max rot err(1-dot)',maxr,'max pos err',maxp); print(worst)
errs={}
for fr in range(0,s['num']):
  for i,bn in enumerate(b):
    p=leaf.get(bn['name'].lower())
    if not p: continue
    P,Q=conv(i,*kk[fr][i],S); u=interp(rot[p],fr/24.0)
    d=abs(sum(Q[j]*u[j] for j in range(4))); errs[bn['name']]=max(errs.get(bn['name'],0),1-min(1,d))
print(sorted(errs.items(),key=lambda x:-x[1])[:12])
