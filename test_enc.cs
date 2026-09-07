using System;
using Microsoft.Data.Sqlite;
class Program {
    static void Main() {
        try {
            var b = new SqliteConnectionStringBuilder { DataSource = "test.db", Password = "secret" };
            using var conn = new SqliteConnection(b.ToString());
            conn.Open();
            Console.WriteLine("Success");
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
}
