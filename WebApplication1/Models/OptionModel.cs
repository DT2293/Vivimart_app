using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Models
{
    public class OptionModelConfig : IEntityTypeConfiguration<OptionModel>
    {
        public void Configure(EntityTypeBuilder<OptionModel> builder)
        {
            builder.HasNoKey();
        }
    }

    public class OptionModel
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
}