using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Text;

namespace dotnetstartermvc.Menu
{
    public class AdminSidebarService
    {
        private readonly IUrlHelper UrlHelper;
        public List<SidebarItem> Items { get; set; } = new List<SidebarItem>();

        public AdminSidebarService(IUrlHelperFactory factory, IActionContextAccessor action)
        {
            UrlHelper = factory.GetUrlHelper(action.ActionContext);

            Items.Add(new SidebarItem() { Type = SidebarItemType.Divider });
            Items.Add(new SidebarItem() { Type = SidebarItemType.Heading, Title = "Quản lý CSDL" });

            Items.Add(new SidebarItem()
            {
                Type = SidebarItemType.NavItem,
                Controller = "DbManage",
                Action = "Index",
                Area = "Database",
                Title = "Quản lý Database",
                AwesomeIcon = "fas fa-database"
            });

            Items.Add(new SidebarItem() { Type = SidebarItemType.Divider });
            Items.Add(new SidebarItem() { Type = SidebarItemType.Heading, Title = "Quản lý thống kê" });

            Items.Add(new SidebarItem()
            {
                Type = SidebarItemType.NavItem,
                Title = "Lịch & biểu đồ",
                AwesomeIcon = "far fa-folder",
                collapseID = "allChartandSchedule",
                Items = new List<SidebarItem>() {
                         new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Chart",
                                Action = "Index",
                                Area = "",
                                Title = "Danh sách biểu đồ"
                        },
                    },
            });

            Items.Add(new SidebarItem() { Type = SidebarItemType.Divider });
            Items.Add(new SidebarItem() { Type = SidebarItemType.Heading, Title = "Quản lý thông tin người dùng" });

            Items.Add(new SidebarItem()
            {
                Type = SidebarItemType.NavItem,
                Title = "Phân quyền, thành viên",
                AwesomeIcon = "far fa-folder",
                collapseID = "role",
                Items = new List<SidebarItem>() {
                        new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Role",
                                Action = "Index",
                                Area = "Identity",
                                Title = "Danh sách vai trò (role)"
                        },
                         new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Role",
                                Action = "Create",
                                Area = "Identity",
                                Title = "Tạo vai trò (role) mới"
                        },
                        new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "User",
                                Action = "Index",
                                Area = "Identity",
                                Title = "Danh sách thành viên"
                        },
                    },
            });
            Items.Add(new SidebarItem() { Type = SidebarItemType.Divider });
            Items.Add(new SidebarItem() { Type = SidebarItemType.Heading, Title = "Quản lý các dịch vụ website" });

            Items.Add(new SidebarItem()
            {
                Type = SidebarItemType.NavItem,
                Title = "Quản lý dịch vụ",
                AwesomeIcon = "far fa-folder",
                collapseID = "service",
                Items = new List<SidebarItem>() {
                        new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Services",
                                Action = "Index",
                                Area = "",
                                Title = "Danh sách dịch vụ"
                        },
                         new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Services",
                                Action = "Create",
                                Area = "",
                                Title = "Tạo dịch vụ"
                        },
                    },
            });

            Items.Add(new SidebarItem()
            {
                Type = SidebarItemType.NavItem,
                Title = "Quản lý thông báo",
                AwesomeIcon = "far fa-folder",
                collapseID = "notification",
                Items = new List<SidebarItem>() {
                        new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Notifications",
                                Action = "Index",
                                Area = "",
                                Title = "Danh sách thông báo"
                        },
                         new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Notifications",
                                Action = "Create",
                                Area = "",
                                Title = "Tạo thông báo"
                        },
                    },
            });

            Items.Add(new SidebarItem()
            {
                Type = SidebarItemType.NavItem,
                Title = "Quản lý tuyển dụng",
                AwesomeIcon = "far fa-folder",
                collapseID = "recruitment",
                Items = new List<SidebarItem>() {
                        new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Recruitments",
                                Action = "Index",
                                Area = "",
                                Title = "Danh sách tuyển dụng"
                        },
                         new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Recruitments",
                                Action = "Create",
                                Area = "",
                                Title = "Tạo tuyển dụng"
                        },
                    },
            });

            Items.Add(new SidebarItem()
            {
                Type = SidebarItemType.NavItem,
                Title = "Quản lý lịch công tác",
                AwesomeIcon = "far fa-folder",
                collapseID = "workschedule",
                Items = new List<SidebarItem>() {
                        new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "WorkSchedules",
                                Action = "Index",
                                Area = "",
                                Title = "Danh sách lịch công tác"
                        },
                         new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "WorkSchedules",
                                Action = "Create",
                                Area = "",
                                Title = "Tạo lịch công tác"
                        },
                    },
            });

            Items.Add(new SidebarItem()
            {
                Type = SidebarItemType.NavItem,
                Title = "Quản lý nhiệm vụ",
                AwesomeIcon = "far fa-folder",
                collapseID = "assignments",
                Items = new List<SidebarItem>() {
                    new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Assignments",
                                Action = "Index",
                                Area = "",
                                Title = "Danh sách nhiệm vụ"
                        },
                         new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Assignments",
                                Action = "Create",
                                Area = "",
                                Title = "Tạo nhiệm vụ"
                        },
                    },
            });

            Items.Add(new SidebarItem()
            {
                Type = SidebarItemType.NavItem,
                Controller = "Contact",
                Action = "Index",
                Area = "Contact",
                Title = "Quản lý liên hệ",
                AwesomeIcon = "far fa-address-card"
            });

            Items.Add(new SidebarItem() { Type = SidebarItemType.Divider });
            Items.Add(new SidebarItem() { Type = SidebarItemType.Heading, Title = "Lịch công tác & nhiệm vụ" });
            Items.Add(new SidebarItem() { Type = SidebarItemType.Heading, Title = "(Dành cho nhân viên)" });

            Items.Add(new SidebarItem()
            {
                Type = SidebarItemType.NavItem,
                Title = "Lịch công tác",
                AwesomeIcon = "far fa-folder",
                collapseID = "Schedule",
                Items = new List<SidebarItem>() {
                        new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "WorkSchedules",
                                Action = "ListSchedule",
                                Area = "",
                                Title = "Tất cả lịch công tác"
                        },
                        new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "WorkSchedules",
                                Action = "FilterByDay",
                                Area = "",
                                Title = "Lịch công tác theo ngày"
                        },
                        new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "WorkSchedules",
                                Action = "FilterByWeek",
                                Area = "",
                                Title = "Lịch công tác theo tuần"
                        },
                    },
            });

            Items.Add(new SidebarItem()
            {
                Type = SidebarItemType.NavItem,
                Title = "Phân công nhiệm vụ",
                AwesomeIcon = "far fa-folder",
                collapseID = "Task",
                Items = new List<SidebarItem>() {
                        new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Assignments",
                                Action = "ListAssignments",
                                Area = "",
                                Title = "Tất cả nhiệm vụ"
                        },
                        new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Assignments",
                                Action = "FilterByDay",
                                Area = "",
                                Title = "Nhiệm vụ theo ngày"
                        },
                        new SidebarItem() {
                                Type = SidebarItemType.NavItem,
                                Controller = "Assignments",
                                Action = "FilterByWeek",
                                Area = "",
                                Title = "Nhiệm vụ theo tuần"
                        },
                    },
            });
        }


        public string renderHtml()
        {
            var html = new StringBuilder();

            foreach (var item in Items)
            {
                html.Append(item.RenderHtml(UrlHelper));
            }


            return html.ToString();
        }

        public void SetActive(string Controller, string Action, string Area)
        {
            foreach (var item in Items)
            {
                if (item.Controller == Controller && item.Action == Action && item.Area == Area)
                {
                    item.IsActive = true;
                    return;
                }
                else
                {
                    if (item.Items != null)
                    {
                        foreach (var childItem in item.Items)
                        {
                            if (childItem.Controller == Controller && childItem.Action == Action && childItem.Area == Area)
                            {
                                childItem.IsActive = true;
                                item.IsActive = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}