namespace ReviewApi.Domain.Dto
{
    public class PaginationDTO
    {

        public int Page { get; protected set; }
        public int QuantityPerPage { get; protected set; }

        public PaginationDTO(int page, int quantityPerPage)
        {
            if (page < 1)
            {
                page = 1; 
            }
            if (quantityPerPage < 1)
            {
                quantityPerPage = 1;
            }
            if (quantityPerPage > 40)
            {
                quantityPerPage = 40; 
            }
            Page = page;
            QuantityPerPage = quantityPerPage;
        }
    }
}
