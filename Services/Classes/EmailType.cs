namespace Services.Classes
{
    public enum EmailType
    {
        None,
        AccountActivation,
        NewCollaborator,
        RemovedCollaborator,
        RemovedListItem,
        MovedListItem,
        AddedListItem,
        NameChange,
        EmailChange,
        VerifyEmail,
        PasswordChange,
        OrderConfirmation,
        Review,
        ResetPassword,
    }
}
