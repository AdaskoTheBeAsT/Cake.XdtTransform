﻿using System.IO;
using System.Text;
using Cake.Core.IO;
using Cake.Testing;
using Cake.XdtTransform.Tests.Properties;
using FluentAssertions;

namespace Cake.XdtTransform.Tests.Fixtures {
    internal sealed class XdtTransformationFixture {
        public IFileSystem FileSystem { get; set; }
        public FilePath SourceFile { get; set; }
        public FilePath TransformFile { get; set; }
        public FilePath TargetFile { get; set; }

        public XdtTransformationFixture(bool sourceFileExists = true, bool transformFileExists = true, bool targetFileExists = false) {
            var environment = FakeEnvironment.CreateUnixEnvironment();
            var fileSystem = new FakeFileSystem(environment);
            fileSystem.CreateDirectory("/Working");

            if (sourceFileExists) {
                var sourceFile = fileSystem.CreateFile("/Working/web.config").SetContent(Resources.XdtTransformation_SourceFile);
                SourceFile = sourceFile.Path;
            }

            if (transformFileExists) {
                var transformFile = fileSystem.CreateFile("/Working/web.release.config").SetContent(Resources.XdtTramsformation_TransformFile);
                TransformFile = transformFile.Path;
            }

            if (targetFileExists) {
                var targetFile = fileSystem.CreateFile("/Working/transformed.config").SetContent(Resources.XdtTransformation_TargetFile);
                TargetFile = targetFile.Path;
            } else {
                TargetFile = "/Working/transformed.config";
            }

            FileSystem = fileSystem;
        }

        public void TransformConfig() {
            XdtTransformation.TransformConfig(FileSystem, SourceFile, TransformFile, TargetFile);
        }

        public XdtTransformationLog TransformConfigWithDefaultLogger() {
            return XdtTransformation.TransformConfigWithDefaultLogger(FileSystem, SourceFile, TransformFile, TargetFile);
        }

        public string GetTargetFileContent() {
            var targetFile = FileSystem.GetFile(TargetFile);
            targetFile.Exists.Should().BeTrue();
            using (var transformedStream = targetFile.OpenRead())
            using (var streamReader = new StreamReader(transformedStream, Encoding.UTF8)) {
                return streamReader.ReadToEnd();
            }
        }
    }
}
