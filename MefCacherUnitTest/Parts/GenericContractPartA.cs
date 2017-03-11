namespace OhNoPub.MefCacherUnitTest.Parts
{
    class GenericContractPartA
        : IGenericContract<string, object>
    {
        object IGenericContract<string, object>.OtherValue => null;

        string IGenericContract<string, object>.Value => null;
    }
}
