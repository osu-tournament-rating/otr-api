using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ApplyRetroactiveDataFetchStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update matches that have been successfully fetched
            migrationBuilder.Sql(@"
                UPDATE matches
                SET data_fetch_status = 2 /* Fetched */
                WHERE start_time IS NOT NULL
            ");

            // Update matches that have been successfully fetched
            migrationBuilder.Sql(@"
                UPDATE matches
                SET data_fetch_status = 3 /* NotFound */
                WHERE start_time IS NULL
            ");

            // Update beatmaps that have been successfully fetched
            migrationBuilder.Sql(@"
                UPDATE beatmaps
                SET data_fetch_status = 2 /* Fetched */
                WHERE diff_name != ''
            ");

            // Update beatmaps that were not found
            migrationBuilder.Sql(@"
                UPDATE beatmaps
                SET data_fetch_status = 3 /* NotFound */
                WHERE diff_name = ''
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reset all data_fetch_status values back to NotFetched (0)
            migrationBuilder.Sql(@"
                UPDATE matches
                SET data_fetch_status = 0 /* NotFetched */
                WHERE data_fetch_status IN (2, 3, 4) /* Fetched, NotFound, Error */
            ");

            migrationBuilder.Sql(@"
                UPDATE beatmaps
                SET data_fetch_status = 0 /* NotFetched */
                WHERE data_fetch_status IN (2, 3, 4) /* Fetched, NotFound, Error */
            ");
        }
    }
}
