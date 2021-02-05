var target = Argument("target", "Default");
var DebugConfiguration = Argument("configuration", "Debug");
var ReleaseConfiguration = Argument("configuration", "Release");
var solutionFolder = "./src";
var projFile = "./src/FilmEditor";
var outputFolder = "./output";
var selfcontainedOutputFolder = $"{ outputFolder }/self-contained";
var dependentOutputFolder = $"{ outputFolder }/framework-dependent";

var callerInfo = Context.GetCallerInfo();
var pwd = callerInfo.SourceFilePath.ToString();
pwd = pwd.Remove(pwd.Length - 11);
pwd = pwd.Replace('/','\\');

Task("Clean")
    .Does(() => {
        CleanDirectory(outputFolder);
        CleanDirectory("./test/300 s01e01");
        CleanDirectory("./test/anime4kcpp");
        CleanDirectory("./test/realsr_ncnn_vulkan");
        CleanDirectory("./test/srmd_ncnn_vulkan");
        CleanDirectory("./test/waifu2x_converter_cpp");
        CleanDirectory("./test/waifu2x_ncnn_vulkan");
    });

Task("Version")
    .IsDependentOn("Clean")
    .Does(() => {
        var propsFile = "./src/Directory.Build.props";
        var readedVersion = XmlPeek(propsFile, "//Version");
        var currentVersion = new Version(readedVersion);
        var newMinor = currentVersion.Minor;

        if (target == "publish")
        {
            newMinor++;
        }

        var semVersion = new Version(currentVersion.Major, newMinor, currentVersion.Build + 1);
        var version = semVersion.ToString();

        XmlPoke(propsFile, "//Version", version);

        Information(version);
    });

Task("Restore")
    .IsDependentOn("Version")
    .Does(() => {
        DotNetCoreRestore(solutionFolder);
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(solutionFolder, new DotNetCoreBuildSettings{
            Configuration = DebugConfiguration,
            NoRestore = true
        });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {
        DotNetCoreTest(solutionFolder, new DotNetCoreTestSettings{
            Configuration = DebugConfiguration,
            NoRestore = true,
            NoBuild = true
        });
    });

Task("Run")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCoreRun(projFile, new DotNetCoreRunSettings{
            Configuration = DebugConfiguration,
            NoRestore = true,
            NoBuild = true
        });
    });

Task("RunScale")
    .IsDependentOn("Test")
    .Does(() => {
        var arguments = new ProcessArgumentBuilder();
        arguments.Append("scale");
        arguments.Append("--new");
        arguments.Append("--exe");
        arguments.Append("\"C:\\Users\\besmi\\Tools\\Video2x\\video2x-nightly-win32-light\\video2x.exe\"");
        arguments.Append("--movie");
        arguments.Append("\"C:\\Users\\besmi\\Development\\FilmEditor\\test\\300 s01e01.mp4\"");
        arguments.Append("--output");
        arguments.Append("\"C:\\Users\\besmi\\Development\\FilmEditor\\test\"");
        DotNetCoreRun(projFile, arguments, new DotNetCoreRunSettings{
            Configuration = DebugConfiguration,
            NoRestore = true,
            NoBuild = true
        });
    });

Task("RunScale-4k")
    .IsDependentOn("Test")
    .Does(() => {
        var arguments = new ProcessArgumentBuilder();
        arguments.Append("scale");
        arguments.Append("--new");
        arguments.Append("--exe");
        arguments.Append("\"C:\\Users\\besmi\\Tools\\Video2x\\video2x-nightly-win32-light\\video2x.exe\"");
        arguments.Append("--movie");
        arguments.Append("\"C:\\Users\\besmi\\Development\\FilmEditor\\test\\300 s01e01.mp4\"");
        arguments.Append("--output");
        arguments.Append("\"C:\\Users\\besmi\\Development\\FilmEditor\\test\"");
        arguments.Append("--4k");
        DotNetCoreRun(projFile, arguments, new DotNetCoreRunSettings{
            Configuration = DebugConfiguration,
            NoRestore = true,
            NoBuild = true
        });
    });

Task("RunScale-4k-noExeFlag")
    .IsDependentOn("Test")
    .Does(() => {
        var arguments = new ProcessArgumentBuilder();
        arguments.Append("scale");
        arguments.Append("--new");
        arguments.Append("--movie");
        arguments.Append($"\"{ pwd }\\test\\300 s01e01.mp4\"");
        arguments.Append("--output");
        arguments.Append($"\"{ pwd }\\test\"");
        arguments.Append("--4k");
        DotNetCoreRun(projFile, arguments, new DotNetCoreRunSettings{
            Configuration = DebugConfiguration,
            NoRestore = true,
            NoBuild = true
        });
    });

Task("RunScale-Anime4kcpp")
    .IsDependentOn("Test")
    .Does(() => {
        var arguments = new ProcessArgumentBuilder();
        arguments.Append("scale");
        arguments.Append("--movie");
        arguments.Append($"\"{ pwd }\\test\\300 s01e01.mp4\"");
        arguments.Append("--output");
        arguments.Append($"\"{ pwd }\\test\\anime4kcpp\"");
        arguments.Append("--4k");
        DotNetCoreRun(projFile, arguments, new DotNetCoreRunSettings{
            Configuration = DebugConfiguration,
            NoRestore = true,
            NoBuild = true
        });
    });

Task("RunScale-waifu2x-caffe")
    .IsDependentOn("Test")
    .Does(() => {
        var arguments = new ProcessArgumentBuilder();
        arguments.Append("scale");
        arguments.Append("--movie");
        arguments.Append($"\"{ pwd }\\test\\300 s01e01.mp4\"");
        arguments.Append("--output");
        arguments.Append($"\"{ pwd }\\test\\waifu2x-caffe\"");
        arguments.Append("--driver");
        arguments.Append("waifu2x_caffe");
        arguments.Append("--4k");
        DotNetCoreRun(projFile, arguments, new DotNetCoreRunSettings{
            Configuration = DebugConfiguration,
            NoRestore = true,
            NoBuild = true
        });
    });

Task("RunScale-waifu2x_converter_cpp")
    .IsDependentOn("Test")
    .Does(() => {
        var arguments = new ProcessArgumentBuilder();
        arguments.Append("scale");
        arguments.Append("--movie");
        arguments.Append($"\"{ pwd }\\test\\300 s01e01.mp4\"");
        arguments.Append("--output");
        arguments.Append($"\"{ pwd }\\test\\waifu2x_converter_cpp\"");
        arguments.Append("--driver");
        arguments.Append("waifu2x_converter_cpp");
        arguments.Append("--4k");
        DotNetCoreRun(projFile, arguments, new DotNetCoreRunSettings{
            Configuration = DebugConfiguration,
            NoRestore = true,
            NoBuild = true
        });
    });

Task("RunScale-waifu2x_ncnn_vulkan")
    .IsDependentOn("Test")
    .Does(() => {
        var arguments = new ProcessArgumentBuilder();
        arguments.Append("scale");
        arguments.Append("--movie");
        arguments.Append($"\"{ pwd }\\test\\300 s01e01.mp4\"");
        arguments.Append("--output");
        arguments.Append($"\"{ pwd }\\test\\waifu2x_ncnn_vulkan\"");
        arguments.Append("--driver");
        arguments.Append("waifu2x_ncnn_vulkan");
        arguments.Append("--4k");
        DotNetCoreRun(projFile, arguments, new DotNetCoreRunSettings{
            Configuration = DebugConfiguration,
            NoRestore = true,
            NoBuild = true
        });
    });

Task("RunScale-srmd_ncnn_vulkan")
    .IsDependentOn("Test")
    .Does(() => {
        var arguments = new ProcessArgumentBuilder();
        arguments.Append("scale");
        arguments.Append("--movie");
        arguments.Append($"\"{ pwd }\\test\\300 s01e01.mp4\"");
        arguments.Append("--output");
        arguments.Append($"\"{ pwd }\\test\\srmd_ncnn_vulkan\"");
        arguments.Append("--driver");
        arguments.Append("srmd_ncnn_vulkan");
        arguments.Append("--4k");
        DotNetCoreRun(projFile, arguments, new DotNetCoreRunSettings{
            Configuration = DebugConfiguration,
            NoRestore = true,
            NoBuild = true
        });
    });

Task("RunScale-realsr_ncnn_vulkan")
    .IsDependentOn("Test")
    .Does(() => {
        var arguments = new ProcessArgumentBuilder();
        arguments.Append("scale");
        arguments.Append("--movie");
        arguments.Append($"\"{ pwd }\\test\\300 s01e01.mp4\"");
        arguments.Append("--output");
        arguments.Append($"\"{ pwd }\\test\\realsr_ncnn_vulkan\"");
        arguments.Append("--driver");
        arguments.Append("realsr_ncnn_vulkan");
        arguments.Append("--4k");
        DotNetCoreRun(projFile, arguments, new DotNetCoreRunSettings{
            Configuration = DebugConfiguration,
            NoRestore = true,
            NoBuild = true
        });
    });

Task("RunScale-All-Drivers")
    .IsDependentOn("RunScale-Anime4kcpp")
    //.IsDependentOn("RunScale-waifu2x-caffe")
    .IsDependentOn("RunScale-waifu2x_converter_cpp")
    .IsDependentOn("RunScale-waifu2x_ncnn_vulkan")
    .IsDependentOn("RunScale-srmd_ncnn_vulkan")
    .IsDependentOn("RunScale-realsr_ncnn_vulkan");

Task("Package")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePack(solutionFolder, new DotNetCorePackSettings{
            Configuration = ReleaseConfiguration,
            OutputDirectory = outputFolder,
            NoRestore = true,
            NoBuild = true
        });
    });

Task("Publish-Win-x64")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projFile, new DotNetCorePublishSettings{
            Configuration = ReleaseConfiguration,
            OutputDirectory = $"{ selfcontainedOutputFolder }/win-x64",
            PublishSingleFile = true,
            SelfContained = true,
            Runtime = "win-x64"
        });
    });

Task("Publish-Linux-x64")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projFile, new DotNetCorePublishSettings{
            Configuration = ReleaseConfiguration,
            OutputDirectory = $"{ selfcontainedOutputFolder }/linux-x64",
            PublishSingleFile = true,
            SelfContained = true,
            Runtime = "linux-x64"
        });
    });

Task("Publish-Osx-x64")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projFile, new DotNetCorePublishSettings{
            Configuration = ReleaseConfiguration,
            OutputDirectory = $"{ selfcontainedOutputFolder }/osx-x64",
            PublishSingleFile = true,
            SelfContained = true,
            Runtime = "osx-x64"
        });
    });

Task("Publish-Win-arm64")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projFile, new DotNetCorePublishSettings{
            Configuration = ReleaseConfiguration,
            OutputDirectory = $"{ selfcontainedOutputFolder }/win-arm64",
            PublishSingleFile = true,
            SelfContained = true,
            Runtime = "win-arm64"
        });
    });

Task("Publish-Win-x86")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projFile, new DotNetCorePublishSettings{
            Configuration = ReleaseConfiguration,
            OutputDirectory = $"{ selfcontainedOutputFolder }/win-x86",
            PublishSingleFile = true,
            SelfContained = true,
            Runtime = "win-x86"
        });
    });

Task("Publish-Dependent-Win-x64")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projFile, new DotNetCorePublishSettings{
            Configuration = ReleaseConfiguration,
            OutputDirectory = $"{ dependentOutputFolder }/win-x64",
            PublishSingleFile = true,
            SelfContained = false,
            Runtime = "win-x64"
        });
    });

Task("Publish-Dependent-Linux-x64")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projFile, new DotNetCorePublishSettings{
            Configuration = ReleaseConfiguration,
            OutputDirectory = $"{ dependentOutputFolder }/linux-x64",
            PublishSingleFile = true,
            SelfContained = false,
            Runtime = "linux-x64"
        });
    });

Task("Publish-Dependent-Osx-x64")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projFile, new DotNetCorePublishSettings{
            Configuration = ReleaseConfiguration,
            OutputDirectory = $"{ dependentOutputFolder }/osx-x64",
            PublishSingleFile = true,
            SelfContained = false,
            Runtime = "osx-x64"
        });
    });

Task("Publish-Dependent-Win-arm64")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projFile, new DotNetCorePublishSettings{
            Configuration = ReleaseConfiguration,
            OutputDirectory = $"{ dependentOutputFolder }/win-arm64",
            PublishSingleFile = true,
            SelfContained = false,
            Runtime = "win-arm64"
        });
    });

Task("Publish-Dependent-Win-x86")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projFile, new DotNetCorePublishSettings{
            Configuration = ReleaseConfiguration,
            OutputDirectory = $"{ dependentOutputFolder }/win-x86",
            PublishSingleFile = true,
            SelfContained = false,
            Runtime = "win-x86"
        });
    });

Task("Publish")
    .IsDependentOn("Publish-Win-x64")
    .IsDependentOn("Publish-Linux-x64")
    .IsDependentOn("Publish-Osx-x64")
    .IsDependentOn("Publish-Win-arm64")
    .IsDependentOn("Publish-Win-x86")
    .IsDependentOn("Publish-Dependent-Win-x64")
    .IsDependentOn("Publish-Dependent-Linux-x64")
    .IsDependentOn("Publish-Dependent-Osx-x64")
    .IsDependentOn("Publish-Dependent-Win-arm64")
    .IsDependentOn("Publish-Dependent-Win-x86");

Task("Publish-Debug-Dependent-Win-x64")
    .IsDependentOn("Test")
    .Does(() => {
        DotNetCorePublish(projFile, new DotNetCorePublishSettings{
            Configuration = DebugConfiguration,
            OutputDirectory = $"{ dependentOutputFolder }/debug/win-x64",
            PublishSingleFile = true,
            SelfContained = false,
            Runtime = "win-x64"
        });
    });

Task("Default")
    .IsDependentOn("Run");

RunTarget(target);