﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Models.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Blog.Core.Models.DAL
{
    public class PostStore: IPostStore
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly SiteSettings _siteSettings;
        
        public List<Post> Posts => GetAllPostsWithNames();

        private string _path => $"{_hostingEnvironment.ContentRootPath}{_siteSettings.PostsFolderPath}/";
        
        public PostStore(IHostingEnvironment hostingEnvironment, IOptions<SiteSettings> siteSettings)
        {
            _hostingEnvironment = hostingEnvironment;
            _siteSettings = siteSettings.Value;
        }
        
        public async Task<string> GetContentByFilename(string name)
        {
            var fullPath = $"{_path}{name}.cshtml";
            string result;
            
            try
            {
                result = await File.ReadAllTextAsync(fullPath);
            }
            catch (Exception e)
            {
                result = await Task.FromResult($"Content removed or unaccessible. Exception is: {e.Message}");
            }

            return result;
        }
        
        private List<Post> GetAllPostsWithNames()
        {
            return Directory.GetFiles(_path, "*.cshtml", SearchOption.AllDirectories)
                            .Select(p => p.Replace(_path, string.Empty).Replace(".cshtml", string.Empty))
                            .Select(p => new Post {Filename = p})
                            .ToList();
        }
    }
}