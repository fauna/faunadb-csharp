namespace FaunaDB.Types
{
    internal static class RefParser
    {
        public static RefV Parse(Value value)
        {
            return value.GetOption(Field.At("@ref")).Match(
                Some: Parse,
                None: () =>
                {
                    var id = value.GetOption(Field.At("id").To<string>());
                    var coll = value.GetOption(Field.At("collection"));
                    var db = value.GetOption(Field.At("database"));

                    return Mk(id, coll, db);
                });
        }

        private static RefV Mk(IOption<string> id, IOption<Value> cls, IOption<Value> db)
        {
            var idE = id.Value;
            var classE = Cast<RefV>(cls);
            var databaseE = Cast<RefV>(db);

            if (classE == null && databaseE == null)
            {
                return Native.FromName(idE);
            }

            return new RefV(id: idE, collection: classE, database: databaseE);
        }

        private static T Cast<T>(IOption<Value> opt)
            where T : RefV =>
            opt.Match(arg => arg as T, () => default(T));
    }
}
