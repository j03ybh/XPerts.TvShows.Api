using TechMinimalists.Database.Sql.Extensions;
using TechMinimalists.Database.Core.Interfaces.Entities;
using XPerts.TvShows.Models;
using TechMinimalists.Database.Core;
using XPerts.TvShows.Database;

namespace XPerts.TvShows.Mappers
{
    public sealed class TvShowGenreMapper : JoinedModelMapper<TvShow, Genre, TvShowGenre>
    {
        
        public TvShowGenreMapper(IModelMapper<TvShow> leftModelMapper, IModelMapper<Genre> rightModelMapper)
            : base(leftModelMapper, rightModelMapper)
        {
        }

        protected override string LeftIdColumnName => TvShowGenreTableConfiguration.TvShowId;

        protected override string RightIdColumnName => TvShowGenreTableConfiguration.GenreId;

        protected override long GetLeftId(TvShowGenre model) => model?.TvShowId ?? 0;

        protected override long GetRightId(TvShowGenre model)=> model?.GenreId ?? 0;

        protected override void SetLeftId(TvShowGenre model, long id)
        {
            if(model is not null)
                model.TvShowId = id;
        }
        protected override void SetRightId(TvShowGenre model, long id)
        {
            if (model is not null)
                model.GenreId = id;
        }

        protected override void SetLeftModel(TvShowGenre model, TvShow leftModel)
        {
            if (model is not null)
                model.TvShow = leftModel;
        }


        protected override void SetRightModel(TvShowGenre model, Genre rightModel)
        {
            if (model is not null)
                model.Genre = rightModel;
        }
    }
}
