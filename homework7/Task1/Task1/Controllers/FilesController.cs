using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MyNUnitLib;
using Task1.Models;

namespace Task1.Controllers
{
    /// <summary>
    /// Controller for assemblies uploading.
    /// </summary>
    public class FilesController : Controller
    {
        /// <summary>
        /// Uploads assemblies, launches tests and redirects to the page with results.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Upload(IFormFileCollection files)
        {
            await using var repository = new Repository();

            var currentTestRun = new TestRun
            {
                Id = Guid.NewGuid(),
                Source = new List<AssemblyFile>(),
                Report = new List<TestInfoModel>()
            };

            if (files != null)
            {
                foreach (var file in files)
                {
                    using var reader = new BinaryReader(file.OpenReadStream());
                    currentTestRun.Source.Add(new AssemblyFile
                    {
                        Id = Guid.NewGuid(),
                        Name = file.FileName,
                        Content = reader.ReadBytes((int) file.Length)
                    });
                }
            }

            var result = MyNUnit.RunTestsAndGetReport(currentTestRun.Source.
                Select(s => s.Content)).
                SelectMany(dict => dict.Value).
                Select(testInfo => new TestInfoModel
                {
                    Id = Guid.NewGuid(),
                    ActualException = testInfo.ActualException?.ToString(),
                    ExpectedException = testInfo.ExpectedException?.ToString(),
                    IsIgnored = testInfo.IsIgnored,
                    IsSuccessful = testInfo.IsSuccessful,
                    MethodName = testInfo.MethodName,
                    ReasonToIgnore = testInfo.ReasonToIgnore,
                    Time = testInfo.Time.ToString()
                })
                .ToList();

            foreach (var testInfoModel in result)
            {
                currentTestRun.Report.Add(testInfoModel);
            }

            await repository.AddAsync(currentTestRun);
            await repository.SaveChangesAsync();

            return RedirectToAction("ListReports", "home");
        }
    }
}
