using AutoMapper;
using Quizzik_Project.Models;

namespace Quizzik_Project.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Question, QuestionDTO>().ReverseMap();
            CreateMap<Option, OptionDTO>().ReverseMap();
            CreateMap<QuizAttempt, QuizAttemptDTO>().ReverseMap();
            CreateMap<Quiz, QuizDTO>().ReverseMap();
            CreateMap<Leaderboard, LeaderboardDTO>().ReverseMap();
        }
    }
}
