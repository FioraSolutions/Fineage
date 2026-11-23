using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

public sealed class OfficialWorldEmitterPlaybackEditModeTests
{
    [SetUp]
    public void SetUp()
    {
        OfficialWorldEmitterTable.ClearForTests();
    }

    [TearDown]
    public void TearDown()
    {
        OfficialWorldEmitterTable.ClearForTests();
    }

    const string LobbyJson = "{\"emitters\":[{\"map\":\"Lobby\",\"name\":\"Emitter5\",\"tileX\":0,\"tileY\":0,\"location\":{\"x\":-9319.4,\"y\":238524.5,\"z\":-6602.0},\"rotation\":{\"x\":0,\"y\":0,\"z\":0},\"drawScale\":0.1,\"drawScale3D\":{\"x\":1,\"y\":1,\"z\":1},\"prefabClass\":\"we_beam\"},{\"map\":\"17_25\",\"name\":\"Emitter2\",\"tileX\":17,\"tileY\":25,\"location\":{\"x\":0,\"y\":0,\"z\":0},\"rotation\":{\"x\":0,\"y\":0,\"z\":0},\"drawScale\":1,\"drawScale3D\":{\"x\":1,\"y\":1,\"z\":1},\"prefabClass\":\"we_a\"}]}";

    [Test]
    public void CollectLobby_returns_lobby_rows_regardless_of_tiles()
    {
        // The additive l2_lobby scene is never the active scene at login and
        // its map is Lobby.unr, so only the contract collect picks these up.
        OfficialWorldEmitterTable.LoadFrom(Encoding.UTF8.GetBytes(LobbyJson), "test://lobby");
        var dest = new List<OfficialWorldEmitterTable.Emitter>();
        OfficialWorldEmitterPlayback.CollectLobby(dest);
        Assert.AreEqual(1, dest.Count);
        Assert.AreEqual("Lobby/Emitter5", dest[0].Id);
        Assert.AreEqual("Data/Effects/world/we_beam/we_beam", dest[0].ResourcesPath);
    }
}
