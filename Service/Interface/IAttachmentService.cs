using BMW_20250523.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMW_20250523.Service.Interface;

public interface IAttachmentService
{
    // Create, Read, Update, Delete (CRUD) operations

    // Create: AddAttachment
    public Attachment AddAttachment(string uri);

    // Read: GetAttachment, GetAllAttachments, GetAttachmentsByIds
    public Attachment GetAttachment(string attachmentId);
    public List<Attachment> GetAllAttachments();
    public List<Attachment> GetAttachmentsByIds(List<string> attachmentIds);

    // Update: ReplaceAttachment
    public void ReplaceAttachment(string attachmentId, Attachment newAttachment);

    // Delete: DeleteAttachment
    public void DeleteAttachment(string attachmentId);

    public List<Attachment> GenerateSampleAttachments(int numberOfAttachments);
}
