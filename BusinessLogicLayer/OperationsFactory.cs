using DataAccessLayer;

namespace BusinessLogicLayer
{
    public static class OperationsFactory
    {
        public static CRUD_OperationsClass GetOperations(int choiceNumber)
        {
            CRUD_OperationsClass ObjSelector = null;

            switch (choiceNumber)
            {
                case 1:
                    ObjSelector = new REST_Operations();
                    break;
                case 2:
                    ObjSelector = new CSOM_Operations();
                    break;
                default:
                    ObjSelector = new REST_Operations();
                    break;
            }
            return ObjSelector;
        }
    }
}
