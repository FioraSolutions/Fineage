using System;
using System.Collections.Generic;

public static partial class NetEnchant
{
    public static Session OpenFor(int scrollObjectId)
    {
        if (scrollObjectId <= 0)
            return default;
        return new Session
        {
            Open = true,
            ScrollObjectId = scrollObjectId,
            TargetObjectId = 0
        };
    }

    public static Session Select(Session session, int targetObjectId)
    {
        if (!session.Open || targetObjectId <= 0 || targetObjectId == session.ScrollObjectId)
            return session;
        session.TargetObjectId = targetObjectId;
        return session;
    }

    public static bool CanConfirm(Session session)
    {
        return session.Open
            && session.ScrollObjectId > 0
            && session.TargetObjectId > 0
            && session.TargetObjectId != session.ScrollObjectId;
    }

    public static View ViewOf(NetEnchantResult result)
    {
        var view = new View
        {
            CloseWindow = true,
            ObjectId = result.ObjectId,
            EnchantLevel = result.EnchantLevel,
            CrystalItemId = result.CrystalItemId,
            CrystalCount = result.CrystalCount
        };
        switch (result.Result)
        {
            case ResultSuccess:
            case ResultKeep:
            case ResultReset:
                view.ShowEnchant = result.ObjectId > 0;
                break;
            case ResultCrystallize:
                view.RemoveItem = result.ObjectId > 0;
                break;
        }

        return view;
    }
}
