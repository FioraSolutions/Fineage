using System.Net;
using System.Net.Sockets;
using Xunit;

namespace Fineage.Playtest.Tests;

public class PlaytestEndpointTests
{
    [Fact]
    public void EmptyInput_DoesNotOverride()
    {
        PlaytestEndpoint endpoint = PlaytestEndpoint.Resolve(new PlaytestEndpointInput());

        Assert.False(endpoint.OverrideLogin);
        Assert.False(endpoint.OverrideGameHost);
        Assert.False(endpoint.OverrideGamePort);
        Assert.Equal(string.Empty, endpoint.LoginHost);
        Assert.Equal(0, endpoint.LoginPort);
    }

    [Fact]
    public void BakedHost_SetsLoginAndGame_AndDefaultsLoginPort()
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

        Assert.Equal("play.example", endpoint.LoginHost);
        Assert.Equal("play.example", endpoint.GameHost);
        Assert.Equal(PlaytestEndpoint.DefaultLoginPort, endpoint.LoginPort);
        Assert.True(endpoint.OverrideLogin);
        Assert.True(endpoint.OverrideGameHost);
        Assert.False(endpoint.OverrideGamePort);
    }

    [Fact]
    public void DistinctHosts_AndExplicitPorts()
    {
        PlaytestEndpoint endpoint = PlaytestEndpoint.Resolve(new PlaytestEndpointInput
        {
            BakedJson = """
                {"host":"shared.example","loginHost":"login.example","gameHost":"","loginPort":"2107","gamePort":4000}
                """,
        });

        Assert.Equal("login.example", endpoint.LoginHost);
        Assert.Equal("shared.example", endpoint.GameHost);
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
    public void CommittedTemplates_DoNotOverride()
    {
        string root = RepoRoot();
        string streaming = File.ReadAllText(Path.Combine(root, "l2-unity/Assets/StreamingAssets/fineage-endpoint.json"));
        string baked = File.ReadAllText(Path.Combine(root, "l2-unity/Assets/Resources/FineagePlaytest.json"));

        PlaytestEndpoint endpoint = PlaytestEndpoint.Resolve(new PlaytestEndpointInput
        {
            StreamingJson = streaming,
            BakedJson = baked,
        });

        Assert.False(endpoint.OverrideLogin);
        Assert.False(endpoint.OverrideGameHost);
        Assert.False(endpoint.OverrideGamePort);
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
