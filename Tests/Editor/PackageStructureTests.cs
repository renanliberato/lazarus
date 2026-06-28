using NUnit.Framework;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Lazarus.Tests
{
    public class PackageStructureTests
    {
        [Test]
        public void Package_Manifest_Exists_And_Is_Valid()
        {
            // Verify package.json exists at package root
            var packagePath = "Packages/com.renanliberato.lazarus";
            var manifestPath = Path.Combine(packagePath, "package.json");

            Assert.IsTrue(File.Exists(manifestPath), $"Package manifest not found at {manifestPath}");

            // Verify it's valid JSON
            var manifestJson = File.ReadAllText(manifestPath);
            var manifest = JsonUtility.FromJson<PackageManifest>(manifestJson);

            Assert.IsNotNull(manifest, "Package manifest could not be parsed as JSON");
            Assert.IsFalse(string.IsNullOrEmpty(manifest.name), "Package manifest missing 'name' field");
            Assert.IsFalse(string.IsNullOrEmpty(manifest.version), "Package manifest missing 'version' field");
        }

        [System.Serializable]
        private class PackageManifest
        {
            public string name;
            public string version;
            public string displayName;
            public string description;
            public string[] dependencies;
        }

        [Test]
        public void Package_Has_Runtime_Assembly()
        {
            var runtimeAsmdefPath = Path.Combine("Packages/com.renanliberato.lazarus/Runtime", "com.renanliberato.lazarus.asmdef");
            Assert.IsTrue(File.Exists(runtimeAsmdefPath), $"Runtime assembly definition not found at {runtimeAsmdefPath}");
        }

        [Test]
        public void Package_Has_Test_Assembly()
        {
            var testAsmdefPath = Path.Combine("Packages/com.renanliberato.lazarus/Tests/Editor", "com.renanliberato.lazarus.Tests.asmdef");
            Assert.IsTrue(File.Exists(testAsmdefPath), $"Test assembly definition not found at {testAsmdefPath}");
        }
    }
}