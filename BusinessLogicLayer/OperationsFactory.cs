using DataAccessLayer;

namespace BusinessLogicLayer
{
    //TODO [CR RT]: Make enum for application operation types. Think in advance, for 2 options int values is okay but what will happen for 10 options? Also, 1 - 2 options do not really say samething specific
    //TODO [CR RT]: Rename ObjSelector/ Give more intuitive name e.g. use naming of returned class 
    //TODO [CR RT]: Add class and methods documentation

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
