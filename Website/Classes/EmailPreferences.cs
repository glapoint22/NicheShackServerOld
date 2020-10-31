namespace Website.Classes
{
    public struct EmailPreferences
    {
        public bool? NameChange { get; set; }
        public bool? EmailChange { get; set; }
        public bool? PasswordChange { get; set; }
        public bool? ProfilePicChange { get; set; }
        public bool? NewCollaborator { get; set; }
        public bool? RemovedCollaborator { get; set; }
        public bool? RemovedListItem { get; set; }
        public bool? MovedListItem { get; set; }
        public bool? AddedListItem { get; set; }
        public bool? ListNameChange { get; set; }
        public bool? DeletedList { get; set; }
        public bool? Review { get; set; }
    }
}
