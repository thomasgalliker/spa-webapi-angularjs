using HomeCinema.Entities;

namespace HomeCinema.Data.Configurations
{
    public class MovieConfiguration : EntityBaseConfiguration<Movie>
    {
        public MovieConfiguration()
        {
            this.Property(m => m.Title).IsRequired().HasMaxLength(100);
            this.Property(m => m.GenreId).IsRequired();
            this.Property(m => m.Director).IsRequired().HasMaxLength(100);
            this.Property(m => m.Writer).IsRequired().HasMaxLength(50);
            this.Property(m => m.Producer).IsRequired().HasMaxLength(50);
            this.Property(m => m.Writer).HasMaxLength(50);
            this.Property(m => m.Producer).HasMaxLength(50);
            this.Property(m => m.Rating).IsRequired();
            this.Property(m => m.Description).IsRequired().HasMaxLength(2000);
            this.Property(m => m.TrailerURI).HasMaxLength(200);
            this.HasMany(m => m.Stocks).WithRequired().HasForeignKey(s => s.MovieId);
        }
    }
}