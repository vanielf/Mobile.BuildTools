﻿using System.Diagnostics;
using System.IO;
using Mobile.BuildTools.Tests.Mocks;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures
{
    public abstract class FixtureBase
    {
        protected ITestOutputHelper _testOutputHelper { get; }
        protected string ProjectDirectory { get; }

        protected FixtureBase(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        protected FixtureBase(string projectDirectory, ITestOutputHelper testOutputHelper)
            : this(testOutputHelper)
        {
            ProjectDirectory = projectDirectory;
        }

        protected TestBuildConfiguration GetConfiguration()
        {
            var stackTrace = new StackTrace();
            var testOutput = Path.Combine("Tests", stackTrace.GetFrame(1).GetMethod().Name);
            if(Directory.Exists(testOutput))
            {
                Directory.Delete(testOutput, true);
            }

            Directory.CreateDirectory(testOutput);
            return new TestBuildConfiguration
            {
                Logger = new XunitLog(_testOutputHelper),
                Platform = Tasks.Utils.Platform.Unsupported,
                IntermediateOutputPath = testOutput,
                ProjectDirectory = ProjectDirectory
            };
        }

        protected static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                if (file.Name == ".DS_Store")
                    continue;

                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // copy subdirectories, copy them and their contents to new location.
            foreach (var subdir in dirs)
            {
                var temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }
    }
}
