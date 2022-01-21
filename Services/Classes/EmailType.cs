namespace Services.Classes
{
    public enum EmailType
    {
        None,
        AccountActivation,
        ResetPassword,
        NameChange,
        EmailChange,
        PasswordChange,
        ProfilePicChange,
        VerifyEmail,
        VerifyAccountDeletion,
        NewCollaborator,
        RemovedCollaborator,
        RemovedListItem,
        MovedListItem,
        AddedListItem,
        ListNameChange,
        DeletedList,
        Review,
        DeleteAccount
    }
}
