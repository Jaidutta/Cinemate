﻿using Cinemate.Services.Interfaces;

namespace Cinemate.Services
{
    public class BasicImageService : IImageService
    {
        private readonly IHttpClientFactory _httpClient;

        public BasicImageService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }
        public string DecodeImage(byte[] poster, string contentType)
        {
            if (poster == null) return null;

            var posterImage = Convert.ToBase64String(poster);
            return $"data: {contentType};base64,{posterImage}";
        }

        public async Task<byte[]> EncodeImageAsync(IFormFile poster)
        {   /* This method can be used to create/edit a custom movie entry or
             * editing a movie that has been imported into the db
             */
            if (poster == null) return null;

            using var ms = new MemoryStream();
            await poster.CopyToAsync(ms);

            return ms.ToArray();

        }

        public async Task<byte[]> EncodeImageURLAsync(string imageURL)
        {
            var client = _httpClient.CreateClient();
            var response = await client.GetAsync(imageURL);

            using Stream stream = await response.Content.ReadAsStreamAsync();

            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            return ms.ToArray();
        }
    }
}
