using System;

namespace MyTemplate.Core.Model
{
    public interface IDatabaseModel<TKey>
    {
        TKey Id { get; set; }
        bool IsValid { get; set; }
    }
}
