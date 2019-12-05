using Confighub.Microsoft.Extensions.Configuration.Api.Models.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Confighub.Microsoft.Extensions.Configuration
{
    internal class JsonConfigurationProvider : FileConfigurationProvider
    {
        private readonly ILoggerFactory _loggerFactory;
        private int _jsonArrayDepth = -1;
        internal JsonConfigurationProvider(FileConfigurationSource source, ILoggerFactory loggerFactory) : base(source)
        {
            _loggerFactory = loggerFactory;
        }

        public override void Load(Stream stream)
        {
            try
            {
                Data = JsonFileParser.Parse(stream, _jsonArrayDepth);
            }
            catch (JsonReaderException e)
            {
                string errorLine = string.Empty;
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    IEnumerable<string> fileContent;
                    using (var streamReader = new StreamReader(stream))
                    {
                        fileContent = ReadLines(streamReader);
                        errorLine = RetrieveErrorContext(e, fileContent);
                    }
                }

                throw new FormatException(Resources.FormatError_JSONParseError(e.LineNumber, errorLine), e);
            }
        }

        internal void SetDepth(int depth)
        {
            _jsonArrayDepth = depth;
        }

        internal void Store(Pull pull)
        {
            var root = JsonSchemaGenerator.Generate(pull, _loggerFactory);
            var json = JsonConvert.SerializeObject(root, Formatting.Indented);
            File.WriteAllText(Source.Path, json);
        }

        private static string RetrieveErrorContext(JsonReaderException e, IEnumerable<string> fileContent)
        {
            string errorLine = null;
            if (e.LineNumber >= 2)
            {
                var errorContext = fileContent.Skip(e.LineNumber - 2).Take(2).ToList();
                if (errorContext.Count() >= 2)
                {
                    errorLine = errorContext[0].Trim() + Environment.NewLine + errorContext[1].Trim();
                }
            }
            if (string.IsNullOrEmpty(errorLine))
            {
                var possibleLineContent = fileContent.Skip(e.LineNumber - 1).FirstOrDefault();
                errorLine = possibleLineContent ?? string.Empty;
            }
            return errorLine;
        }

        private static IEnumerable<string> ReadLines(StreamReader streamReader)
        {
            string line;
            do
            {
                line = streamReader.ReadLine();
                yield return line;
            } while (line != null);
        }
    }
}
