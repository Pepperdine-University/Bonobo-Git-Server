﻿using Bonobo.Git.Server.App_GlobalResources;
using Bonobo.Git.Server.Attributes;
using Bonobo.Git.Server.Data;
using LibGit2Sharp;
using Microsoft.IdentityModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Bonobo.Git.Server.Models
{
    public class RepositoryModel : INameProperty
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public bool AnonymousAccess { get; set; }
        public RepositoryPushMode AllowAnonymousPush { get; set; }
        public UserModel[] Users { get; set; }
        public UserModel[] Administrators { get; set; }
        public TeamModel[] Teams { get; set; }
        public bool AuditPushUser { get; set; }
        public byte[] Logo { get; set; }
        public bool RemoveLogo { get; set; }
        public string LinksRegex { get; set; }
        public string LinksUrl { get; set; }
        public bool LinksUseGlobal { get; set; }
        public List<ServiceAccount> ServiceAccounts { get; set; }
        public List<Dependency> Dependencies { get; set; }
        public IList<KnownDependency> KnownDependencies { get; set; }

        public RepositoryModel()
        {
            AllowAnonymousPush = RepositoryPushMode.Global;
            LinksUseGlobal = true;
            LinksUrl = "";
            LinksRegex = "";
        }

        public bool NameIsValid
        {
            get
            {
                // Check for an exact match, not just a substring hit - based on RegularExpressionAttribute
                Match match = Regex.Match(Name, NameValidityRegex);
                return match.Success && match.Index == 0 && match.Length == Name.Length;
            }
        }

        public string DisplayName
        {
            get
            {
                return Name;
            }
        }

        public void EnsureCollectionsAreValid()
        {
            if (Administrators == null)
            {
                Administrators = new UserModel[0];
            }

            if (Users == null)
            {
                Users = new UserModel[0];
            }

            if (Teams == null)
            {
                Teams = new TeamModel[0];
            }
        }

        public const string NameValidityRegex = @"([\w\.-])*([\w])$";
    }

    //TODO: Implement IValidatable Object Abstract Class
    //https://stackoverflow.com/questions/61049966/how-to-notify-users-for-custom-validation-based-on-multiple-properties
    public class RepositoryDetailModel : IValidatableObject
    {
        public RepositoryDetailModel()
        {
            AllowAnonymousPush = RepositoryPushMode.Global;
            LinksUseGlobal = true;
        }

        public Guid Id { get; set; }

        [Remote("UniqueNameRepo", "Validation", AdditionalFields="Id", ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Duplicate_Name")]
        [UniqueRepoName]
        [RegularExpression(RepositoryModel.NameValidityRegex, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_FileName_Regex")]
        [FileName(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_FileName")]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_StringLength")]
        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_Name")]
        public string Name { get; set; }

        [AllowHtml]
        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_Group")]
        [StringLength(255, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_StringLength")]
        public string Group { get; set; }

        [AllowHtml]
        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_Description")]
        [StringLength(255, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_StringLength")]
        public string Description { get; set; }



        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_Users")]
        public UserModel[] Users { get; set; }

        [AllowHtml]
        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_ServiceAccounts")]
        public List<ServiceAccount> ServiceAccounts { get; set; }

        //[DependencyValidator]
        [AllowHtml]
        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_Dependencies")]
        public List<Dependency> Dependencies { get; set; }

        [AllowHtml]
        public IList<KnownDependency> KnownDependencies { get; set; }

        public Guid[] PostedSelectedUsers { get; set; }
        public UserModel[] AllUsers { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_Teams")]
        public TeamModel[] Teams { get; set; }
        public Guid[] PostedSelectedTeams { get; set; }
        public TeamModel[] AllTeams { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_Administrators")]
        public UserModel[] Administrators { get; set; }
        public Guid[] PostedSelectedAdministrators { get; set; }
        public UserModel[] AllAdministrators { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_IsCurrentUserAdmin")]
        public bool IsCurrentUserAdministrator { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_Anonymous")]
        public bool AllowAnonymous { get; set; }

        [EnumDataType(typeof(RepositoryPushMode), ErrorMessageResourceType=typeof(Resources), ErrorMessageResourceName="Repository_Edit_InvalidAnonymousPushMode")]
        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_AllowAnonymousPush")]
        public RepositoryPushMode AllowAnonymousPush { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_Status")]
        public RepositoryDetailStatus Status { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_AuditPushUser")]
        public bool AuditPushUser { get; set; }

        public RepositoryLogoDetailModel Logo { get; set; }
        public string GitUrl { get; set; }
        public string PersonalGitUrl { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_AreRegexLinksVisible")]
        public bool AreRegexLinksVisible { get; set; }

        [Remote("IsValidRegex", "Validation")]
        [IsValidRegex]
        [Display(ResourceType = typeof(Resources), Name = "Settings_Global_LinksRegex")]
        public string LinksRegex { get; set; }
        [Display(ResourceType = typeof(Resources), Name = "Settings_Global_LinksUrl")]
        public string LinksUrl { get; set; }
        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_LinksUseGlobal")]
        public bool LinksUseGlobal { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            RepositoryDetailModel model = (RepositoryDetailModel)validationContext.ObjectInstance;
            List<ValidationResult> results = new List<ValidationResult>();

            if (model.Dependencies != null && model.Dependencies.Count() > 0)
            {
                List<ValidationResult> dependencyErrors = ValidateDependencies(model.Dependencies);

                if (dependencyErrors.Count() > 0)
                {
                    foreach(ValidationResult error in dependencyErrors)
                    {
                        results.Add(error);
                    }
                }
            }
            
            if (model.ServiceAccounts != null && model.ServiceAccounts.Count() > 0)
            {
                List<ValidationResult> serviceAccountErrors = ValidateServiceAccounts(model.ServiceAccounts);

                if (serviceAccountErrors.Count() > 0)
                {
                    foreach (ValidationResult error in serviceAccountErrors)
                    {
                        results.Add(error);
                    }
                }
            }

            if (results.Count() > 0)
            {
                return results;
            }

            return new List<ValidationResult> { ValidationResult.Success };
        }

        public List<ValidationResult> ValidateDependencies(List<Dependency> dependencies)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            //Gets all KnownDependencies from the database to cross-reference names
            IRepositoryRepository RepositoryRepository = DependencyResolver.Current.GetService<IRepositoryRepository>();
            List<KnownDependency> knownDependencies = RepositoryRepository.GetAllKnownDependencies().ToList();
            List<Dependency> newDependencies = dependencies.Where(d => d.Id == Guid.Empty).ToList();
            List<string> knownDependencyNames = knownDependencies.Select(kd => kd.ComponentName).ToList();

            //Get a list of dependencies with a Future Date
            List<Dependency> futureDateErrorDependencies = dependencies
                                                            .Where(dependency => dependency.DateUpdated > DateTime.Today)
                                                            .ToList();
            //Handle any dependencies with a future date
            if (futureDateErrorDependencies != null && futureDateErrorDependencies.Count() > 0)
            {
                List<string> memberNames = new List<string>();
                foreach (Dependency dependency in futureDateErrorDependencies)
                {
                    int index = dependencies.IndexOf(dependency);
                    memberNames.Add($"Dependencies[{index}].DateUpdated");
                }

                string errorMessage = Resources.Dependency_Future_Date;
                results.Add(new ValidationResult(errorMessage, memberNames));
            }

            //Loop over new dependencies and handle errors
            foreach (Dependency newDependency in newDependencies)
            {
                //Set the ComponentName to null if there is a KnownDependency Id selected
                if (newDependency.KnownDependenciesId != Guid.Empty)
                {
                    newDependency.KnownDependency.ComponentName = null;
                }

                //Check for duplicate KnownDependency Ids
                bool duplicateKnownDependencies = dependencies
                                                    .Where(d => newDependency.KnownDependenciesId != Guid.Empty &&
                                                                d.KnownDependenciesId == newDependency.KnownDependenciesId)
                                                    .Count() > 1;
                if (duplicateKnownDependencies)
                {
                    List<string> memberNames = new List<string>();
                    List<int> indices = dependencies.Select((item, index) => new { Item = item, Index = index })
                                    .Where(o => o.Item.KnownDependenciesId == newDependency.KnownDependenciesId)
                                    .Select(d => d.Index).ToList();

                    foreach (int index in indices)
                    {
                        if (dependencies[index].Id == Guid.Empty)
                        {
                            memberNames.Add($"Dependencies[{index}].KnownDependenciesId");
                        }
                    }

                    //Make sure this error hasn't been handled already for the same dependencies
                    bool isDuplicateError = results.Where(r => r.ErrorMessage == Resources.Known_Dependency_CantHaveDuplicates && r.MemberNames.All(memberNames.Contains) && memberNames.All(r.MemberNames.Contains)).Count() > 0;

                    if (!isDuplicateError)
                    {
                        results.Add(new ValidationResult(Resources.Known_Dependency_CantHaveDuplicates, memberNames));
                    }
                }

                string knownDependencyName = newDependency.KnownDependency.ComponentName != null ? newDependency.KnownDependency.ComponentName.Trim().ToLower() : null;

                //Check for duplicate ComponentNames
                if (knownDependencyName != null && knownDependencyNames.Where(name => name.Trim().ToLower() == knownDependencyName).Count() > 0)
                {
                    List<string> memberNames = new List<string>();
                    List<Dependency> dependenciesWithExistingComponentName = newDependencies
                                                                                .Where(d => 
                                                                                        d.KnownDependency.ComponentName != null && 
                                                                                        d.KnownDependency.ComponentName.Trim().ToLower() == knownDependencyName)
                                                                                .ToList();
                    foreach (Dependency dependencyWithExistingComponentName in dependenciesWithExistingComponentName)
                    {
                        if (dependencyWithExistingComponentName.Id == Guid.Empty)
                        {
                            int index = dependencies.IndexOf(dependencyWithExistingComponentName);
                            if (index != -1)
                            {
                                memberNames.Add($"Dependencies[{index}].KnownDependency.ComponentName");
                            }
                        }
                    }

                    //Make sure this error hasn't been handled already for the same dependencies
                    bool isDuplicateError = results.Where(r => r.ErrorMessage == Resources.Dependency_Duplicate_Names && r.MemberNames.All(memberNames.Contains) && memberNames.All(r.MemberNames.Contains)).Count() > 0;

                    if (!isDuplicateError)
                    {
                        results.Add(new ValidationResult(Resources.Known_Dependency_CantHaveDuplicateNames, memberNames));
                    }
                }

                //Check for duplicate ComponentNames in newly-added dependencies
                if (knownDependencyName != null && dependencies.Where(d => d.KnownDependency.ComponentName != null && d.KnownDependency.ComponentName.Trim().ToLower() == knownDependencyName).Count() > 1)
                {
                    List<string> memberNames = new List<string>();
                    List<Dependency> dependenciesWithDuplicateComponentName = dependencies.Where(d => d.KnownDependency.ComponentName.Trim().ToLower() == knownDependencyName).ToList();
                    foreach (Dependency dependencyWithDuplicateComponentName in dependenciesWithDuplicateComponentName)
                    {
                        if (dependencyWithDuplicateComponentName.Id == Guid.Empty)
                        {
                            int index = dependencies.IndexOf(dependencyWithDuplicateComponentName);
                            if (index != -1)
                            {
                                memberNames.Add($"Dependencies[{index}].KnownDependency.ComponentName");
                            }
                        }
                    }

                    //Make sure this error hasn't been handled already for the same dependencies
                    bool isDuplicateError = results.Where(r => r.ErrorMessage == Resources.Known_Dependency_CantHaveDuplicates && r.MemberNames.All(memberNames.Contains) && memberNames.All(r.MemberNames.Contains)).Count() > 0;

                    if (!isDuplicateError)
                    {
                        results.Add(new ValidationResult(Resources.Known_Dependency_CantHaveDuplicates, memberNames));
                    }
                }
            }

            return results;
        }

        public List<ValidationResult> ValidateServiceAccounts(List<ServiceAccount> serviceAccounts)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            List<string> serviceAccountNames = serviceAccounts.Select(sa => sa.ServiceAccountName).ToList();
            List<ServiceAccount> newServiceAccounts = serviceAccounts.Where(sa => sa.Id == Guid.Empty).ToList();

            List<ServiceAccount> futureDateErrorServiceAccounts = serviceAccounts
                                                            .Where(sa => sa.PassLastUpdated > DateTime.Today)
                                                            .ToList();

            if (futureDateErrorServiceAccounts != null && futureDateErrorServiceAccounts.Count() > 0)
            {
                List<string> memberNames = new List<string>();
                foreach (ServiceAccount serviceAccount in futureDateErrorServiceAccounts)
                {
                    int index = serviceAccounts.IndexOf(serviceAccount);
                    memberNames.Add($"ServiceAccounts[{index}].PassLastUpdated");
                }

                string errorMessage = Resources.ServiceAccount_Future_Date;
                results.Add(new ValidationResult(errorMessage, memberNames));
            }

            foreach (ServiceAccount serviceAccount in newServiceAccounts)
            {
                List<ServiceAccount> duplicateServiceAccounts = serviceAccounts.Where(sa => sa.ServiceAccountName.Trim().ToLower() == serviceAccount.ServiceAccountName.Trim().ToLower()).ToList();
                if (duplicateServiceAccounts.Count() > 1)
                {
                    List<string> memberNames = new List<string>();

                    foreach(ServiceAccount duplicateServiceAccount in duplicateServiceAccounts)
                    {
                        int index = serviceAccounts.IndexOf(duplicateServiceAccount);
                        if (duplicateServiceAccount.Id == Guid.Empty && index != -1)
                        {
                            memberNames.Add($"ServiceAccounts[{index}].ServiceAccountName");
                        }
                    }

                    bool isDuplicateError = results.Where(r => r.ErrorMessage == Resources.ServiceAccount_Duplicate_Name && r.MemberNames.All(memberNames.Contains) && memberNames.All(r.MemberNames.Contains)).Count() > 0;

                    if (!isDuplicateError)
                    {
                        results.Add(new ValidationResult(Resources.ServiceAccount_Duplicate_Name, memberNames));
                    }
                }
            }

            if (results.Count() > 0)
            {
                return results;
            }

            return new List<ValidationResult> { ValidationResult.Success };
        }
    }

    public enum RepositoryDetailStatus
    {
        Unknown = 0,
        Valid,
        Missing
    }

    public class RepositoryTreeDetailModel
    {
        [Display(ResourceType = typeof(Resources), Name = "Repository_Tree_Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Tree_CommitMessage")]
        public string CommitMessage { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Tree_CommitDate")]
        public DateTime? CommitDate { get; set; }
        public string CommitDateString { get { return CommitDate.HasValue ? CommitDate.Value.ToString() : CommitDate.ToString(); } }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Tree_Author")]
        public string Author { get; set; }
        public bool IsTree { get; set; }
        public bool IsLink { get; set; }
        public string TreeName { get; set; }
        public bool IsImage { get; set; }
        public bool IsText { get; set; }
        public bool IsMarkdown { get; set; }
        public string Path { get; set; }
        public byte[] Data { get; set; }
        public string Text { get; set; }
        public string TextBrush { get; set; }
        public string GitUrl { get; set; }
        public string PersonalGitUrl { get; set; }
        public Encoding Encoding { get; set; }
        public RepositoryLogoDetailModel Logo { get; set; }
    }

    public class RepositoryTreeModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Branch { get; set; }
        public string Path { get; set; }
        public string Readme { get; set; }
        public string GitUrl { get; set; }
        public string PersonalGitUrl { get; set; }
        public RepositoryLogoDetailModel Logo { get; set; }
        public IEnumerable<RepositoryTreeDetailModel> Files { get; set; }
        public List<ServiceAccount> ServiceAccounts { get; set; }
        public List<Dependency> Dependencies { get; set; }
    }

    public class RepositoryCommitsModel
    {
        public string Name { get; set; }
        public RepositoryLogoDetailModel Logo { get; set; }
        public string GitUrl { get; set; }
        public string PersonalGitUrl { get; set; }
        public IEnumerable<RepositoryCommitModel> Commits { get; set; }
    }

    public class RepositoryCommitChangeModel
    {
        public string ChangeId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public ChangeKind Status { get; set; }
        public int LinesAdded { get; set; }
        public int LinesDeleted { get; set; }
        public int LinesChanged { get { return LinesAdded + LinesDeleted; } }
        public string Patch { get; set; }
    }

    public class RepositoryCommitNoteModel
    {
        public RepositoryCommitNoteModel(string message, string @namespace)
        {
            this.Message = message;
            this.Namespace = @namespace;
        }

        public string Message { get; set; }

        public string Namespace { get; set; }
    }

    public class RepositoryCommitModel
    {
        public RepositoryCommitModel()
        {
            Links = new List<string>();
        }

        public string Name { get; set; }
        public RepositoryLogoDetailModel Logo { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Commit_ID")]
        public string ID { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Commit_TreeID")]
        public string TreeID { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Commit_Parents")]
        public string[] Parents { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Commit_Author")]
        public string Author { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Commit_AuthorEmail")]
        public string AuthorEmail { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Commit_AuthorAvatar")]
        public string AuthorAvatar { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Commit_Date")]
        public DateTime Date { get; set; }

        private string _message;
        [Display(ResourceType = typeof(Resources), Name = "Repository_Commit_Message")]

        public string Message
        {
            get
            {
                if (String.IsNullOrEmpty(_message))
                {
                    return Resources.Repository_Commit_NoMessageDeclared;
                }
                else
                {
                    return _message;
                }
            }
            set
            {
                _message = value;
            }
        }

        public string MessageShort { get; set; }

        public IEnumerable<string> Tags { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Commit_Changes")]
        public IEnumerable<RepositoryCommitChangeModel> Changes { get; set; }

        public IEnumerable<RepositoryCommitNoteModel> Notes { get; set; }

        public IEnumerable<string> Links { get; set; }
    }

    public class RepositoryBlameModel
    {
        public string Name { get; set; }
        public string TreeName { get; set; }
        public string Path { get; set; }
        public RepositoryLogoDetailModel Logo { get; set; }
        public long FileSize { get; set; }
        public long LineCount { get; set; }
        public IEnumerable<RepositoryBlameHunkModel> Hunks { get; set; }
    }

    public class RepositoryBlameHunkModel
    {
        public RepositoryCommitModel Commit { get; set; }
        public string[] Lines { get; set; }
    }

    public class RepositoryLogoDetailModel
    {
        byte[] _data;

        public RepositoryLogoDetailModel() { }

        public RepositoryLogoDetailModel(byte[] data)
        {
            this._data = data;
        }

        [FileUploadExtensions(Extensions = "PNG,JPG,JPEG,GIF")]
        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_Logo_PostedFile")]
        public HttpPostedFileWrapper PostedFile { get; set; }

        [Display(ResourceType = typeof(Resources), Name = "Repository_Detail_RemoveLogo")]
        public bool RemoveLogo { get; set; }

        public bool Exists
        {
            get
            {
                return BinaryData != null;
            }
        }

        public byte[] BinaryData
        {
            get
            {
                if (_data == null && PostedFile != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Image originalImage = Image.FromStream(PostedFile.InputStream, true, true);

                        int logoWidth = originalImage.Width >= 72 ? 72 : 36;

                        Image resizedImage = originalImage.GetThumbnailImage(logoWidth, (logoWidth * originalImage.Height) / originalImage.Width, null, IntPtr.Zero);

                        resizedImage.Save(ms, ImageFormat.Png);

                        _data = ms.GetBuffer();
                    }
                }

                return _data;
            }
        }

        public string Base64Image
        {
            get
            {
                if (_data != null)
                    return Convert.ToBase64String(_data);

                return null;
            }
        }
    }
}
