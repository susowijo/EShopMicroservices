﻿using FluentValidation;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductCommand(string Name, string Description, string ImageFile, decimal Price, List<string> Category)
        : ICommand<CreateProductResult>;

    public record CreateProductResult(Guid Id);

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price is required");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is required");
        }
    }

    public class CreateProductCommandHandler
        (IDocumentSession session, ILogger<CreateProductCommandHandler> logger)
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("CreateProductCommandHandler.Handle called with {@Query}", command);

            //Create product entity from command object
            var product = new Product
            {
                Name = command.Name,
                Category = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price,
            };

            // Save to database
            session.Store(product);
            await session.SaveChangesAsync();

            return new CreateProductResult(product.Id);
        }
    }
}
