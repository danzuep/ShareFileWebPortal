using ShareFile.Api.Client.Models;
using ShareFile.Api.Helpers.Models;
using Data.ShareFile.Models;
using ShareFileWebPortal.Data.ViewModels;
using AutoMapper;
using Data.ShareFile.DbFirstModels;

namespace ShareFileWebPortal.Profiles
{
    public class ShareFileProfile : Profile
    {
        public ShareFileProfile()
        {   //            source    -->    target
            CreateMap<ShareFileApiGroup, ShareFileDbGroup>();
            CreateMap<ShareFileApiGroup, ShareFileGroupVM>();
            CreateMap<ShareFileApiGroupMember, ShareFileDbGroupMember>()
                .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.MemberUid));
            CreateMap<ShareFileApiGroupMember, ShareFileGroupMemberVM>();

            CreateMap<ShareFileDbGroup, ShareFileGroupVM>().ReverseMap();
            CreateMap<ShareFileDbGroupMember, ShareFileApiGroupMember>().ReverseMap();

            CreateMap<Item, SfItem>()
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.Parent.Id))
                .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Id));
            CreateMap<Folder, SfItem>()
                .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.FileCount))
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.Parent.Id))
                .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Id));
            CreateMap<SfItem, SfItemVM>().ReverseMap();

            CreateMap<Principal, SfPrincipal>()
                .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Id));
            CreateMap<SfPrincipal, SfPrincipalVM>().ReverseMap();
            CreateMap<Contact, SfContact>()
                .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.Count))
                .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Id));
            CreateMap<Group, SfGroup>()
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Contacts))
                .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.NumberOfContacts))
                .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Id));

            CreateMap<Principal, DbPrincipal>()
                .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Id));
            CreateMap<DbPrincipal, SfPrincipalVM>().ReverseMap();

            CreateMap<AccessControl, SfAccessControl>()
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.Item.Id))
                //.ForMember(dest => dest.ItemPath, opt => opt.MapFrom(src => src.Item.Path))
                //.ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item.Name))
                .ForMember(dest => dest.PrincipalId, opt => opt.MapFrom(src => src.Principal.Id))
                .ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Id.GetHashCode().ToString()));
            CreateMap<AccessControl, SFUserItemSecurity>()
                //.ForMember(dest => dest.Uid, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PrincipalUid, opt => opt.MapFrom(src => src.Principal.Id))
                //.ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Principal.Email))
                .ForMember(dest => dest.FolderPath, opt => opt.MapFrom(src => src.Item.SemanticPath))
                .ForMember(dest => dest.FolderName, opt => opt.MapFrom(src => src.Item.Name))
                .ForMember(dest => dest.FolderUid, opt => opt.MapFrom(src => src.Item.Id));
            CreateMap<SfAccessControl, SFUserItemSecurity>()
                .ForMember(dest => dest.PrincipalUid, opt => opt.MapFrom(src => src.PrincipalId))
                .ForMember(dest => dest.PrincipalName, opt => opt.MapFrom(src => src.Principal.Name))
                //src.Principal != null ? src.Principal.Name : src.Group != null ? src.Group.Name : ""))
                .ForMember(dest => dest.PrincipalEmail, opt => opt.MapFrom(src => src.Principal.Email))
                .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Item.ItemCount))
                .ForMember(dest => dest.ItemSizeInKB, opt => opt.MapFrom(src => src.Item.FileSizeInKB))
                .ForMember(dest => dest.FolderPath, opt => opt.MapFrom(src => src.Item.SemanticPath))
                .ForMember(dest => dest.FolderName, opt => opt.MapFrom(src => src.Item.Name))
                .ForMember(dest => dest.FolderUid, opt => opt.MapFrom(src => src.ItemId));
            CreateMap<SfAccessControl, SfAccessControlVM>().ReverseMap();
            CreateMap<SfAccessControl, SfItemVM>().IncludeMembers(m => m.Item)
                .ForMember(dest => dest.PrincipalUid, opt => opt.MapFrom(src => src.PrincipalId))
                .ForMember(dest => dest.PrincipalName, opt => opt.MapFrom(src => src.Principal.Name));
            CreateMap<SfItem, SFUserItemSecurity>()
                .ForMember(dest => dest.FolderUid, opt => opt.MapFrom(src => src.Uid))
                .ForMember(dest => dest.FolderPath, opt => opt.MapFrom(src => src.SemanticPath))
                .ForMember(dest => dest.FolderName, opt => opt.MapFrom(src => src.Name));
        }
    }
}
