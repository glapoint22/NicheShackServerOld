using Website.Classes;

namespace Website.ViewModels
{
    public class ListViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TotalItems { get; set; }
        public string OwnerName { get; set; }
        public ProfilePicInfo ProfilePic { get; set; }
        public string CollaborateId { get; set; }
        public ListPermissions ListPermissions { get; set; }
        public int CollaboratorCount { get; set; }
        public bool IsOwner { get; set; }
    }
}
