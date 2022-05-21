using System;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace server.Models;

public class Access<T>
    where T : Entity<T>
{
    IMongoCollection<T> coll;
    public Access()
    {
        var client = new MongoClient("mongodb+srv://fallguys:UVQp3cB33J9UuAzf@cluster0.cpxch.mongodb.net/test?authSource=admin&replicaSet=atlas-11etck-shard-0&readPreference=primary&appname=MongoDB%20Compass&ssl=true");
        var db = client.GetDatabase("fallguys");
        this.coll = db.GetCollection<T>(typeof(T).Name);
    }

    public async Task Add(T obj)
        => await this.coll.InsertOneAsync(obj);
    
    public async Task Update(T obj)
        => await this.coll.ReplaceOneAsync(x => x.Id == obj.Id, obj);

    public async Task<IEnumerable<T>> Where(Expression<Func<T, bool>> filter)
        => await this.coll.Find(filter).ToListAsync();
}