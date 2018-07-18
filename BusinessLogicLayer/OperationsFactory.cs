namespace BusinessLogicLayer
{
    using Common.Constants;
    using DataAccessLayer;
    //TODO [CR RT]: Add class and methods documentation

    public static class OperationsFactory
    {
        public static BaseListReferenceProvider GetOperations(ApplicationEnums.ListReferenceProviderType choice)
        {
            BaseListReferenceProvider listReferenceProvider = null;

            switch (choice)
            {
                case ApplicationEnums.ListReferenceProviderType.REST:
                    listReferenceProvider = new RestListReferenceProvider();
                    break;
                case ApplicationEnums.ListReferenceProviderType.CSOM:
                    listReferenceProvider = new CsomListReferenceProvider();
                    break;
                default:
                    listReferenceProvider = new RestListReferenceProvider();
                    break;
            }

            return listReferenceProvider;
        }
    }
}