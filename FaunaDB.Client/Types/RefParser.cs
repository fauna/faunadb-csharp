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
                    var cls = value.GetOption(Field.At("class"));
                    var db = value.GetOption(Field.At("database"));

                    return Mk(id, cls, db);
                }
            );
        }

        static RefV Mk(IOption<string> id, IOption<Value> cls, IOption<Value> db)
        {
            var idE = id.Value;
            var classE = Cast<RefV>(cls);
            var databaseE = Cast<DatabaseV>(db);

            if (classE == null && databaseE == null)
                return BuiltIn.FromName(idE);

            if (BuiltIn.DATABASES == classE)
                return new DatabaseV(id: idE, database: databaseE);

            if (BuiltIn.CLASSES == classE)
                return new ClassV(id: idE, database: databaseE);

            if (BuiltIn.INDEXES == classE)
                return new IndexV(id: idE, database: databaseE);

            if (BuiltIn.FUNCTIONS == classE)
                return new FunctionV(id: idE, database: databaseE);

            if (BuiltIn.KEYS == classE)
                return new KeyV(id: idE, database: databaseE);

            return new RefV(id: idE, @class: classE, database: databaseE);
        }

        static T Cast<T>(IOption<Value> opt) where T : RefV =>
            opt.Match(arg => arg as T, () => default(T));
    }
}
