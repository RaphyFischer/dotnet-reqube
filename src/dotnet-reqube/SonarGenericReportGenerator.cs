﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ReQube.Logging;
using ReQube.Models.ReSharper;
using ReQube.Models.SonarQube;
using ReQube.Models.SonarQube.Generic;
using Serilog;
using Constants = ReQube.Models.Constants;

namespace ReQube
{
    public class SonarGenericReportGenerator : SonarBaseReportGenerator
    {
        private new ILogger Logger { get; } = LoggerFactory.GetLogger();

        public override List<ISonarReport> Generate(Report reSharperReport)
        {
            var reportIssueTypes = reSharperReport.IssueTypes.ToDictionary(t => t.Id, type => type);
            var lastLoadedFilePath = "";
            var lastLoadedFileContent = "";
            string[] lastLoadedFileLines = null;

            var sonarQubeReports = new List<ISonarReport>();

            foreach (var project in reSharperReport.Issues)
            {
                var sonarQubeReport = new SonarGenericReport { ProjectName = project.Name };
                var replaceFileNameRegex = new Regex($@"^{Regex.Escape(project.Name)}\\");
                foreach (var issue in project.Issue)
                {
                    if (!reportIssueTypes.TryGetValue(issue.TypeId, out ReportIssueType issueType))
                    {
                        Logger.Information("Unable to find issue type {0}.", issue.TypeId);
                        continue;
                    }

                    if (!Constants.ReSharperToSonarQubeSeverityMap.TryGetValue(
                        issueType.Severity, out string sonarQubeSeverity))
                    {
                        Logger.Information("Unable to map ReSharper severity {0} to SonarQube", issueType.Severity);
                        continue;
                    }

                    ReadIssueFile(issue.File, ref lastLoadedFilePath, ref lastLoadedFileContent, ref lastLoadedFileLines);

                    var line = ((ISonarReportGenerator)this).GetSonarLine(issue.Line);
                    var (startColumn, endColumn) = FindLineOffset(issue.Offset, line, lastLoadedFilePath, lastLoadedFileContent, lastLoadedFileLines);

                    var sonarQubeIssue = new Issue
                    {
                        EngineId = Constants.EngineId,
                        RuleId = issue.TypeId,
                        Type = Constants.SonarQubeCodeSmellType,
                        Severity = sonarQubeSeverity,
                        PrimaryLocation = new PrimaryLocation
                        {
                            FilePath = replaceFileNameRegex.Replace(issue.File, string.Empty),
                            Message = issue.Message,
                            TextRange = new TextRange
                            {
                                StartLine = line,
                                EndLine = line,

                                StartColumn = startColumn,
                                EndColumn = endColumn
                            }
                        }
                    };

                    sonarQubeReport.Issues.Add(sonarQubeIssue);
                }

                sonarQubeReports.Add(sonarQubeReport);
            }

            return sonarQubeReports;
        }
    }
}
