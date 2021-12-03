namespace api.Mappers
{
    public static class EnumMappers
    {
        public static Entities.EState ToEntityStateEnumMapper(this Models.EState? state)
        {
            return state switch
            {
                Models.EState.Rejected => Entities.EState.Rejected,
                Models.EState.Pending => Entities.EState.Pending,
                _ => Entities.EState.Approved
            };
        }
    }
}