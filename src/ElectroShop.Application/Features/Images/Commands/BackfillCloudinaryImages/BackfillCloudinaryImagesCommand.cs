using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Images.Commands.BackfillCloudinaryImages;

public record BackfillCloudinaryImagesCommand : IRequest<Result<BackfillCloudinaryImagesResultDto>>;
