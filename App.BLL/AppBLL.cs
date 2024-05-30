using App.BLL.Services;
using App.Contracts.BLL;
using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.DAL.EF;
using AutoMapper;
using Base.BLL;
using DAL.App.EF;

namespace App.BLL;

public class AppBLL: BaseBLL<AppDbContext>, IAppBLL
{
    private readonly IMapper _mapper;
    private readonly IAppUnitOfWork _uow;
    
    public AppBLL(IAppUnitOfWork uoW, IMapper mapper) : base(uoW)
    {
        _mapper = mapper;
        _uow = uoW;
    }

    private IPetService? _pet;
    private IBlogPostService? _blogPost;
    private IBlogTagService? _blogTag;
    private IExcerciseService? _excercise;
    private IHealthRecordService? _healthRecord;
    private IPostTagService? _postTag;
    private IVeterinaryPracticeService? _veterinaryPractice;
    private IBlogPostCommentService? _postComment;
    private IExcerciseRatingService? _excerciseRating;
    private IVeterinaryPracticeRatingService? _veterinaryPracticeRating;
    private IRefreshTokenService? _tokenService;
    public IAppointmentService? Appointment { get; }
    
    public IPetService Pet => _pet ?? new PetService(_uow, _uow.Pet, _mapper);
    
    public IBlogPostService BlogPost => _blogPost ?? new BlogPostService(_uow, _uow.BlogPost, _mapper);
    public IBlogTagService BlogTag => _blogTag ?? new BlogTagService(_uow, _uow.BlogTag, _mapper);
    public IExcerciseService Excercise => _excercise ?? new ExcerciseService(_uow, _uow.Excercise, _mapper);
    public IHealthRecordService HealthRecord => _healthRecord ?? new HealthRecordService(_uow, _uow.HealthRecord, _mapper);
    public IPostTagService PostTag => _postTag ?? new PostTagService(_uow, _uow.PostTag, _mapper);
    public IVeterinaryPracticeService VeterinaryPractice => _veterinaryPractice ?? new VeterinaryPracticeService(_uow, _uow.VeterinaryPractice, _mapper);
    public IRefreshTokenService AppRefreshTokens => _tokenService ?? new RefreshTokenService(_uow, _uow.RefreshTokens, _mapper);

    public IBlogPostCommentService BlogPostComment =>
        _postComment ?? new BlogPostCommentService(_uow, _uow.BlogPostComment, _mapper);

    public IExcerciseRatingService ExcerciseRating =>
        _excerciseRating ?? new ExcerciseRatingService(_uow, _uow.ExcerciseRating, _mapper);
    
    public IVeterinaryPracticeRatingService VeterinaryPracticeRating => _veterinaryPracticeRating ??
                                                                        new VeterinaryPracticeRatingService(_uow,
                                                                            _uow.VeterinaryPracticeRating, _mapper);




}