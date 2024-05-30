using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.EF;
using App.Domain.Identity;
using AutoMapper;
using Base.Contracts.DAL;
using Base.DAL.EF;
using DAL.App.EF.Repositories;
using DAL.Contracts.Repositories;


namespace DAL.App.EF;

public class AppUOW : BaseUnitOfWork<AppDbContext>, IAppUnitOfWork
{
    private readonly IMapper _mapper;
    public AppUOW(AppDbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }

    private IPetRepository? _pet;
    private IAppointmentRepository? _appointment;
    private IBlogPostRepository? _blogPost;
    private IExcerciseRepository? _excercise;
    private IHealthRecordRepository? _healtRecord;
    private IPostTagRepository? _postTag;
    private IVeterinaryPracticeRepository? _veterinaryPractice;
    private IBlogTagRepository? _tagRepository;
    private IBlogPostCommentRepository? _blogPostComment { get; }
    private IExcerciseRatingRepository? _excerciseRating { get; }
    private IVeterinaryPracticeRatingRepository? _veterinaryPracticeRating { get; }


    private IEntityRepository<AppUser>? _users;
    private IRefreshTokenRepository? _refreshTokens;

    public IEntityRepository<AppUser> Users => _users ??
                                               new BaseEntityRepository<AppUser, AppUser, AppDbContext>(UowDbContext,
                                                   new DalDomainMapper<AppUser, AppUser>(_mapper));
    public IPetRepository Pet => _pet ?? new PetRepository(UowDbContext, _mapper);
    public IAppointmentRepository Appointment => _appointment ?? new AppointmentRepository(UowDbContext, _mapper);
    public IBlogPostRepository BlogPost => _blogPost ?? new BlogPostRepository(UowDbContext, _mapper);
    public IBlogTagRepository BlogTag => _tagRepository ?? new BlogTagRepository(UowDbContext, _mapper);
    public IExcerciseRepository Excercise => _excercise ?? new ExcerciseRepository(UowDbContext, _mapper);
    public IHealthRecordRepository HealthRecord => _healtRecord ?? new HealthRecordRepository(UowDbContext, _mapper);
    public IPostTagRepository PostTag => _postTag ?? new PostTagsRepository(UowDbContext, _mapper);
    public IVeterinaryPracticeRepository VeterinaryPractice => _veterinaryPractice ?? new VeterinaryPracticeRepository(UowDbContext, _mapper);
    public IBlogPostCommentRepository BlogPostComment => _blogPostComment ?? new BlogPostCommentRepository(UowDbContext, _mapper);
    public IExcerciseRatingRepository ExcerciseRating => _excerciseRating ?? new ExcerciseRatingRepository(UowDbContext, _mapper);

    public IVeterinaryPracticeRatingRepository VeterinaryPracticeRating => _veterinaryPracticeRating ??
                                                                           new VeterinaryPracticeRatingRepository(
                                                                               UowDbContext, _mapper);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ?? new IdentityRepo(UowDbContext, _mapper);


}
