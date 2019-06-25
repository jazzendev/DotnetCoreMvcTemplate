using MyTemplate.Domain.Portal.Model;
using MyTemplate.Web.Core.Models;

namespace MyTemplate.Web.Portal
{
    public static class Sitemap
    {
        public static SitemapNode Map { get; private set; }

        static Sitemap()
        {
            Map = new SitemapNode()
            {
                Controller = "Home",
                Action = "Index",
                Title = "首页",
                IsLink = true,
                IconClass = "fa fa-home"
            };
            
            //Users
            Map.AddChild(new SitemapNode() { Controller = "Customer", Action = "Index", Title = "客户信息管理", IconClass = "fas fa-file-alt" }
                         .AddChild(new SitemapNode() { Controller = "Customer", Action = "Detail", Title = "客户详情", IsShow = false })
                         .AddChild(new SitemapNode() { Controller = "Customer", Action = "Create", Title = "添加新客户", IsShow = false })
                         .AddChild(new SitemapNode() { Controller = "Customer", Action = "Edit", Title = "编辑客户", IsShow = false })
                        );

            //Projects
            Map.AddChild(new SitemapNode() { Controller = "Project", Action = "", Title = "项目管理", IsLink = false, IconClass = "fas fa-clipboard-list" }
                             .AddChild(new SitemapNode() { Controller = "Project", Action = "Index", Title = "项目列表", IconClass = "fas fa-clipboard-list" }
                             .AddChild(new SitemapNode() { Controller = "Project", Action = "Detail", Title = "项目详情", IsShow = false }
                                .AddChild(new SitemapNode() { Controller = "WorkItem", Action = "Create", Title = "添加WorkItem", IsShow = false }))
                             .AddChild(new SitemapNode() { Controller = "Project", Action = "Upsert", Title = "项目信息管理", IsShow = false }))
                         .AddChild(new SitemapNode() { Controller = "Project", Action = "Status", Title = "项目进度", IconClass = "fas fa-clipboard-check" })
                         );

            //Tasks
            Map.AddChild(new SitemapNode() { Controller = "WorkItem", Action = "", Title = "任务管理", IsLink = false, IconClass = "fas fa-tasks" }
                            .AddChild(new SitemapNode() { Controller = "WorkItem", Action = "Index", Title = "任务列表", IconClass = "fas fa-thumbtack" })
                        );

            //Tags
            //Map.AddChild(new SitemapNode() { Controller = "Tag", Action = "Index", Title = "标签管理", IconClass = "fas fa-tags" });

            //Portal Admin
            Map.AddChild(new SitemapNode() { Controller = "Login", Action = "Index", Title = "团队成员管理", IconClass = "fa fa-user", VisibleRoles=new PortalRoles[] { PortalRoles.SuperAdmin }  });
        }

        public static SitemapNode GetSitemap()
        {
            return Map;
        }
    }
}
