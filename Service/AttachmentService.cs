using BMW_20250523.Model;
using BMW_20250523.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Service;

public class AttachmentService : IAttachmentService
{
    private readonly List<Attachment> _attachments = [];

    // Create, Read, Update, Delete (CRUD) operations

    // Create: AddAttachment
    public Attachment AddAttachment(string uri)
    {
        if (string.IsNullOrEmpty(uri)) 
            throw new ArgumentException("URI cannot be null or empty.", nameof(uri));

        var newAttachment = new Attachment
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Uri = uri,
            CreatedAt = DateTime.UtcNow,
        };

        do
        {
            var existingAttachment = _attachments.FirstOrDefault(x => x.Id == newAttachment.Id);
            if (existingAttachment == null) break;

            newAttachment.Id = Guid.NewGuid().ToString("N")[..8];
        } while (true);

        _attachments.Add(newAttachment);

        return newAttachment;
    }

    // Read: GetAttachment, GetAllAttachments, GetAttachmentsByIds
    public Attachment GetAttachment(string attachmentId) => _attachments.FirstOrDefault(x => x.Id == attachmentId)
        ?? throw new KeyNotFoundException($"Attachment with ID {attachmentId} not found.");

    public List<Attachment> GetAllAttachments() => _attachments.ToList();

    public List<Attachment> GetAttachmentsByIds(List<string> attachmentIds)
    {
        if (attachmentIds == null || attachmentIds.Count == 0) return _attachments.ToList();
        return _attachments.Where(x => attachmentIds.Contains(x.Id)).ToList();
    }

    // Update: ReplaceAttachment
    public void ReplaceAttachment(string attachmentId, Attachment newAttachment)
    {
        var index = _attachments.FindIndex(x => x.Id == attachmentId);
        if (index != -1) _attachments[index] = newAttachment;
        else throw new KeyNotFoundException($"Attachment with ID {attachmentId} not found.");
    }

    // Delete: DeleteAttachment
    public void DeleteAttachment(string attachmentId)
    {
        var index = _attachments.FindIndex(x => x.Id == attachmentId);
        if (index != -1) _attachments.RemoveAt(index);
        else throw new KeyNotFoundException($"Attachment with ID {attachmentId} not found.");
    }

    public List<Attachment> GenerateSampleAttachments(int numberOfAttachments)
    {
        var attachments = new List<Attachment>();
        if(numberOfAttachments > 0)
        {
            for (int i = 1; i <= numberOfAttachments; i++)
            {
                var seed = Guid.NewGuid().ToString("N")[..8];
                var width = Random.Shared.Next(384, 640);
                var height = Random.Shared.Next(384, 640);

                var uri = $"https://picsum.photos/seed/{seed}/{width}/{height}";

                attachments.Add(AddAttachment(uri));
            }
        }
        return attachments;
    }
}
