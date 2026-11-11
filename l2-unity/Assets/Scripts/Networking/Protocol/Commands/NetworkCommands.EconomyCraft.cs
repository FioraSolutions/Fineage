using System.Collections.Generic;
using UnityEngine;

public sealed class RequestRecipeBookDestroyCommand : INetworkCommand
{
    public int RecipeId;

    public RequestRecipeBookDestroyCommand(int recipeId)
    {
        RecipeId = recipeId;
    }
}

public sealed class RequestRecipeItemMakeInfoCommand : INetworkCommand
{
    public int RecipeId;

    public RequestRecipeItemMakeInfoCommand(int recipeId)
    {
        RecipeId = recipeId;
    }
}

public sealed class RequestRecipeItemMakeSelfCommand : INetworkCommand
{
    public int RecipeId;

    public RequestRecipeItemMakeSelfCommand(int recipeId)
    {
        RecipeId = recipeId;
    }
}

public sealed class RequestRecipeBookOpenCommand : INetworkCommand
{
    public int IsDwarven;

    public RequestRecipeBookOpenCommand(int isDwarven)
    {
        IsDwarven = isDwarven;
    }
}

public sealed class RequestAcquireSkillCommand : INetworkCommand
{
    public int SkillId;
    public int SkillLevel;
    public int SkillType;

    public RequestAcquireSkillCommand(int skillId, int skillLevel, int skillType)
    {
        SkillId = skillId;
        SkillLevel = skillLevel;
        SkillType = skillType;
    }
}

public sealed class RequestAcquireSkillInfoCommand : INetworkCommand
{
    public int SkillId;
    public int SkillLevel;
    public int SkillType;

    public RequestAcquireSkillInfoCommand(int skillId, int skillLevel, int skillType)
    {
        SkillId = skillId;
        SkillLevel = skillLevel;
        SkillType = skillType;
    }
}

public sealed class RequestAnswerJoinPartyCommand : INetworkCommand
{
    public int Response;
    /// <summary>Fineage Net inviter object id from <see cref="AskJoinPartyDto.RequestorObjectId"/>; 0 on Interlude.</summary>
    public int RequestorObjectId;

    public RequestAnswerJoinPartyCommand(int response, int requestorObjectId = 0)
    {
        Response = response;
        RequestorObjectId = requestorObjectId;
    }
}

public sealed class RequestJoinPartyCommand : INetworkCommand
{
    public int TargetObjectId;
    public int ItemDistribution;

    public RequestJoinPartyCommand(int targetObjectId, int itemDistribution = 0)
    {
        TargetObjectId = targetObjectId;
        ItemDistribution = itemDistribution;
    }
}

public sealed class RequestWithdrawalPartyCommand : INetworkCommand
{
}

public sealed class RequestOustPartyMemberCommand : INetworkCommand
{
    public int TargetObjectId;

    public RequestOustPartyMemberCommand(int targetObjectId)
    {
        TargetObjectId = targetObjectId;
    }
}

public sealed class AnswerTradeRequestCommand : INetworkCommand
{
    public int Response;
    /// <summary>Fineage Net requestor object id; zero on Interlude wire.</summary>
    public int RequestorObjectId;

    public AnswerTradeRequestCommand(int response, int requestorObjectId = 0)
    {
        Response = response;
        RequestorObjectId = requestorObjectId;
    }
}

public sealed class RequestTradeCommand : INetworkCommand
{
    public int TargetObjectId;

    public RequestTradeCommand(int targetObjectId)
    {
        TargetObjectId = targetObjectId;
    }
}

public sealed class TradeDoneCommand : INetworkCommand
{
    public int Response;

    public TradeDoneCommand(int response)
    {
        Response = response;
    }
}

public sealed class AddTradeItemCommand : INetworkCommand
{
    public int Trade;
    public int ObjectId;
    public int Count;

    public AddTradeItemCommand(int trade, int objectId, int count)
    {
        Trade = trade;
        ObjectId = objectId;
        Count = count;
    }
}

public sealed class RequestPreviewItemCommand : INetworkCommand
{
    public int ListId;
    public List<Product> BuyList;

    public RequestPreviewItemCommand(int listId, List<Product> buyList)
    {
        ListId = listId;
        BuyList = buyList;
    }
}

