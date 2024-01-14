using System;
using System.IO;
using System.Linq;
using System.Text;

namespace NetVips.Samples;

public class GenerateEnums : ISample
{
    public string Name => "Generate enums";
    public string Category => "Internal";

    private string RemovePrefix(string enumStr)
    {
        const string prefix = "Vips";

        return enumStr.StartsWith(prefix) ? enumStr[prefix.Length..] : enumStr;
    }

    /// <summary>
    /// Generate the `Enums.Generated.cs` file.
    /// </summary>
    /// <remarks>
    /// This is used to generate the `Enums.Generated.cs` file (<see cref="Enums"/>).
    /// </remarks>
    /// <returns>The `Enums.Generated.cs` as string.</returns>
    private string Generate()
    {
        var allEnums = NetVips.GetEnums();

        const string preamble = """
                                //------------------------------------------------------------------------------
                                // <auto-generated>
                                //     This code was generated by a tool.
                                //     libvips version: {0}
                                //
                                //     Changes to this file may cause incorrect behavior and will be lost if
                                //     the code is regenerated.
                                // </auto-generated>
                                //------------------------------------------------------------------------------
                                """;

        var stringBuilder =
            new StringBuilder(string.Format(preamble,
                $"{NetVips.Version(0)}.{NetVips.Version(1)}.{NetVips.Version(2)}"));
        stringBuilder.AppendLine()
            .AppendLine()
            .AppendLine("using System;")
            .AppendLine()
            .AppendLine("namespace NetVips;")
            .AppendLine()
            .AppendLine("public static class Enums")
            .AppendLine("{")
            .AppendLine("    #region auto-generated enums")
            .AppendLine();

        foreach (var name in allEnums)
        {
            var gtype = NetVips.TypeFromName(name);
            var csharpName = RemovePrefix(name);

            stringBuilder.AppendLine("    /// <summary>")
                .AppendLine($"    /// {csharpName}")
                .AppendLine("    /// </summary>")
                .AppendLine($"    public enum {csharpName}")
                .AppendLine("    {");

            var enumValues = NetVips.ValuesForEnum(gtype);
            for (var i = 0; i < enumValues.Count; i++)
            {
                var kvp = enumValues.ElementAt(i);
                var enumKey = kvp.Key.Replace('-', '_').ToPascalCase();

                stringBuilder.AppendLine($"        /// <summary>{enumKey}</summary>")
                    .Append($"        {enumKey} = {kvp.Value}")
                    .AppendLine((i != enumValues.Count - 1 ? "," : string.Empty) + $" // \"{kvp.Key}\"");
            }

            stringBuilder.AppendLine("    }").AppendLine();
        }

        stringBuilder.AppendLine("    #endregion")
            .Append('}');

        return stringBuilder.ToString();
    }

    public void Execute(string[] args)
    {
        File.WriteAllText("Enums.Generated.cs", Generate());

        Console.WriteLine("See Enums.Generated.cs");
    }
}