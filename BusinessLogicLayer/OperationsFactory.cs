namespace BusinessLogicLayer
{
    using Common.Constants;
    using DataAccessLayer;

    /// <summary>
    /// Implements the FactoryPattern for BaseListReferenceProvider
    /// </summary>
    public static class OperationsFactory
    {
        /// <summary>
        /// Returns a specific instance of BaseListReferenceProvider from a given enum elements choice
        /// </summary>
        /// <param name="choice"></param>
        /// <returns></returns>
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