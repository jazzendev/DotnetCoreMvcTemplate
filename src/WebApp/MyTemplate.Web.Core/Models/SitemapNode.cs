using MyTemplate.Domain.Portal.Model;
using System;
using System.Collections.Generic;

namespace MyTemplate.Web.Core.Models
{
    public class SitemapNode
    {
        public string Id
        {
            get { return $"{this.Controller}/{this.Action}"; }
        }
        public string ParentId { get; private set; }

        private string _controller;
        private string _action;
        public string Controller { get { return _controller; } set { _controller = value.ToLower(); } }
        public string Action { get { return _action; } set { _action = value.ToLower(); } }

        public string Title { get; set; }
        public string Description { get; set; }
        public string IconClass { get; set; }

        public PortalRoles[] VisibleRoles { get; set; }

        public bool IsLink { get; set; } = true;
        public bool IsShow { get; set; } = true;
        public List<SitemapNode> Children { get { return _children; } }

        private List<SitemapNode> _children = new List<SitemapNode>();

        public SitemapNode AddChild(SitemapNode child)
        {
            child.ParentId = this.Id;
            _children.Add(child);
            return this;
        }
    }
}
