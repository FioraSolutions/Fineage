using System.Net;
using System.Net.Sockets;
using Xunit;

namespace Fineage.Playtest.Tests;

public class PlaytestEndpointTests
{
    [Fact]
    public void EmptyInput_UsesCloudflarePlaytest()
    {
        PlaytestEndpoint endpoint = PlaytestEndpoint.Resolve(new PlaytestEndpointInput());

        Assert.Equal(PlaytestEndpoint.DefaultPublicHost, endpoint.PublicHost);
        Assert.Equal(PlaytestEndpoint.DefaultLoginHost, endpoint.LoginHost);
        Assert.Equal(PlaytestEndpoint.DefaultGameHost, endpoint.GameHost);
        Assert.Equal(PlaytestEndpoint.DefaultLoginPort, endpoint.LoginPort);
        Assert.Equal(PlaytestEndpoint.DefaultGamePort, endpoint.GamePort);
        Assert.True(endpoint.OverrideLogin);
        Assert.True(endpoint.OverrideGameHost);
        Assert.True(endpoint.OverrideGamePort);
    }

    [Fact]
    public void PublicHost_DoesNotRetargetLoginOrGame()
    {
        PlaytestEndpoint endpoint = PlaytestEndpoint.Resolve(new PlaytestEndpointInput
        {
            BakedJson = """
                {
                  "host": " play.example ",
                  "loginHost": "",
                  "gameHost": "",
                  "loginPort": 0,
                  "gamePort": 0
                }
                """,
        });

        Assert.Equal("play.example", endpoint.PublicHost);
        Assert.Equal(PlaytestEndpoint.DefaultLoginHost, endpoint.LoginHost);
        Assert.Equal(PlaytestEndpoint.DefaultGameHost, endpoint.GameHost);
        Assert.Equal(PlaytestEndpoint.DefaultLoginPort, endpoint.LoginPort);
        Assert.Equal(PlaytestEndpoint.DefaultGamePort, endpoint.GamePort);
    }

    [Fact]
    public void DistinctHosts_AndExplicitPorts()
    {
        PlaytestEndpoint endpoint = PlaytestEndpoint.Resolve(new PlaytestEndpointInput
        {
            BakedJson = """
                {"host":"shared.example","loginHost":"login.example","gameHost":"game.example","loginPort":"2107","gamePort":4000}
                """,
        });

        Assert.Equal("shared.example", endpoint.PublicHost);
        Assert.Equal("login.example", endpoint.LoginHost);
        Assert.Equal("game.example", endpoint.GameHost);
        Assert.Equal(2107, endpoint.LoginPort);
        Assert.Equal(4000, endpoint.GamePort);
        Assert.True(endpoint.OverrideGamePort);
    }

    [Fact]
    public void HigherLayers_OverrideOnlyTheFieldsTheySet()
    {
        var endpoint = PlaytestEndpoint.Resolve(new PlaytestEndpointInput
        {
            BakedJson = """{"host":"baked.example","loginPort":2107}""",
            StreamingJson = "{not json",
            Environment = new Dictionary<string, string>
            {
                ["FINEAGE_PUBLIC_HOST"] = "env.example",
                ["FINEAGE_GAME_PORT"] = "7777",
            },
            SidecarJson = """{"host":"side.example"}""",
            PersistentJson = """{"loginHost":"persist.example"}""",
            CommandLine = new[] { "Fineage.exe", "-fineageGameHost", "cmd.example", "--fineage-login-port=2200" },
        });

        Assert.Equal("side.example", endpoint.PublicHost);
        Assert.Equal("persist.example", endpoint.LoginHost);
        Assert.Equal("cmd.example", endpoint.GameHost);
        Assert.Equal(2200, endpoint.LoginPort);
        Assert.Equal(7777, endpoint.GamePort);
    }

    [Fact]
    public void InvalidJson_IsIgnored()
    {
        PlaytestEndpointFields fields;
        Assert.False(PlaytestEndpoint.TryParseJson("{", out fields));
        Assert.False(PlaytestEndpoint.TryParseJson("   ", out fields));
    }

    [Fact]
    public void Json_AllowsWhitespaceEscapesAndTrailingComma()
    {
        const string json = """
            {
              "host": "edge.example",
              "loginHost": "log\nin",
              "gamePort": 0,
            }
            """;

        Assert.True(PlaytestEndpoint.TryParseJson(json, out PlaytestEndpointFields fields));
        Assert.Equal("edge.example", fields.Host);
        Assert.Equal("log\nin", fields.LoginHost);
        Assert.True(fields.GamePortSet);
        Assert.Equal(0, fields.GamePort);
    }

    [Fact]
    public void CommittedTemplates_MatchCloudflareDefaults()
    {
        string root = RepoRoot();
        string streaming = File.ReadAllText(Path.Combine(root, "l2-unity/Assets/StreamingAssets/fineage-endpoint.json"));
        string baked = File.ReadAllText(Path.Combine(root, "l2-unity/Assets/Resources/FineagePlaytest.json"));

        PlaytestEndpoint endpoint = PlaytestEndpoint.Resolve(new PlaytestEndpointInput
        {
            StreamingJson = streaming,
            BakedJson = baked,
        });

        Assert.Equal(PlaytestEndpoint.DefaultPublicHost, endpoint.PublicHost);
        Assert.Equal(PlaytestEndpoint.DefaultLoginHost, endpoint.LoginHost);
        Assert.Equal(PlaytestEndpoint.DefaultGameHost, endpoint.GameHost);
        Assert.Equal(PlaytestEndpoint.DefaultLoginPort, endpoint.LoginPort);
        Assert.Equal(PlaytestEndpoint.DefaultGamePort, endpoint.GamePort);
        Assert.Contains(PlaytestEndpoint.DefaultLoginHost, streaming);
        Assert.Contains(PlaytestEndpoint.DefaultGameHost, baked);
    }

    [Fact]
    public void CommandLine_EqualsForm_AndSkipsUnknownFlags()
    {
        PlaytestEndpointFields fields = PlaytestEndpoint.ParseCommandLine(new[]
        {
            "Fineage",
            "-projectPath",
            "l2-unity",
            "--fineage-host=edge.example",
            "-fineageGamePort",
            "4000",
        });

        Assert.Equal("edge.example", fields.Host);
        Assert.Equal(4000, fields.GamePort);
        Assert.True(fields.GamePortSet);
    }

    [Theory]
    [InlineData("127.0.0.1")]
    [InlineData("::1")]
    public void EndpointAddress_ParsesLiteral(string host)
    {
        IPAddress address = EndpointAddress.Resolve(host);
        Assert.Equal(IPAddress.Parse(host), address);
    }

    [Fact]
    public void EndpointAddress_ResolvesLocalhostToIpv4()
    {
        IPAddress address = EndpointAddress.Resolve("localhost");
        Assert.Equal(AddressFamily.InterNetwork, address.AddressFamily);
        Assert.True(IPAddress.IsLoopback(address));
    }

    [Fact]
    public void EndpointAddress_RejectsEmptyAndUnresolvable()
    {
        Assert.Throws<ArgumentException>(() => EndpointAddress.Resolve("  "));
        Assert.Throws<InvalidOperationException>(() => EndpointAddress.Resolve("not a host"));
    }

    static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "tools/Fineage.Tools.sln")))
                return dir.FullName;
            dir = dir.Parent;
        }

        throw new InvalidOperationException("repo root not found from " + AppContext.BaseDirectory);
    }
}
