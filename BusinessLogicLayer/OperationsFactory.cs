namespace BusinessLogicLayer
{
    using Common.ApplicationEnums;
    using DataAccessLayer;

    /// <summary>
    ///     Implements the FactoryPattern for BaseListReferenceProvider
    /// </summary>
    public static class OperationsFactory
    {
        /// <summary>
        ///     Returns a specific instance of BaseListReferenceProvider from a given enum elements choice
        /// </summary>
        /// <param name="choice"></param>
        /// <returns></returns>
        public static BaseListReferenceProvider GetOperations(ListReferenceProviderType choice)
        {
            BaseListReferenceProvider listReferenceProvider = null;

            switch (choice)
            {
                case ListReferenceProviderType.Rest:
                    listReferenceProvider = new RestListReferenceProvider();
                    break;
                case ListReferenceProviderType.Csom:
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