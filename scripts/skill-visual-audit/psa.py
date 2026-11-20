import struct
def read_psa(path):
    f=open(path,'rb').read(); p=0; out={}
    while p<len(f):
        cid=f[p:p+20].split(b'\0')[0].decode(); ds,dc=struct.unpack_from('<ii',f,p+24); p+=32
        out[cid]=(p,ds,dc); p+=ds*dc
    bones=[];p,ds,dc=out['BONENAMES']
    for i in range(dc):
        o=p+i*ds; name=f[o:o+64].split(b'\0')[0].decode()
        flags,nch,parent=struct.unpack_from('<iii',f,o+64)
        q=struct.unpack_from('<4f',f,o+76); pos=struct.unpack_from('<3f',f,o+92)
        bones.append(dict(name=name,parent=parent,q=q,pos=pos))
    anims=[];p,ds,dc=out['ANIMINFO']
    for i in range(dc):
        o=p+i*ds; name=f[o:o+64].split(b'\0')[0].decode()
        tb,ri,kcs,kq,kr,tt,rate,sb,first,num=struct.unpack_from('<iiiiffffii',f,o+128)
        anims.append(dict(name=name,bones=tb,tracktime=tt,rate=rate,first=first,num=num))
    kp,kds,kdc=out['ANIMKEYS']
    def keys(a):
        nb=len(bones); res=[]
        for fr in range(a['num']):
            row=[]
            for b in range(nb):
                o=kp+((a['first']+fr)*nb+b)*kds
                pos=struct.unpack_from('<3f',f,o); q=struct.unpack_from('<4f',f,o+12)
                row.append((pos,q))
            res.append(row)
        return res
    return bones,anims,keys
