using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VNTour.Data;

public partial class TourDuLichContext : DbContext
{
    public TourDuLichContext()
    {
    }

    public TourDuLichContext(DbContextOptions<TourDuLichContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDatTour> ChiTietDatTours { get; set; }

    public virtual DbSet<DanhGium> DanhGia { get; set; }

    public virtual DbSet<DanhMucTour> DanhMucTours { get; set; }

    public virtual DbSet<DatTour> DatTours { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<LienHe> LienHes { get; set; }

    public virtual DbSet<MaGiamGium> MaGiamGia { get; set; }

    public virtual DbSet<NgayKhoiHanh> NgayKhoiHanhs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    public virtual DbSet<TourHinhAnh> TourHinhAnhs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-EUOBO6JQ;Initial Catalog=TourDuLich;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDatTour>(entity =>
        {
            entity.HasKey(e => e.Idctdt).HasName("PK__ChiTietD__0F8780C1D99DC1FB");

            entity.ToTable("ChiTietDatTour");

            entity.Property(e => e.Idctdt).HasColumnName("IDCTDT");

            entity.HasOne(d => d.IdDatTourNavigation).WithMany(p => p.ChiTietDatTours)
                .HasForeignKey(d => d.IdDatTour)
                .HasConstraintName("FK_CTDT_DatTour");
        });

        modelBuilder.Entity<DanhGium>(entity =>
        {
            entity.HasKey(e => e.IdDanhGia).HasName("PK__DanhGia__81F722D21B03C6DB");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.NgayTao).HasColumnType("datetime");

            entity.HasOne(d => d.IdKhachHangNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.IdKhachHang)
                .HasConstraintName("FK_DanhGia_KhachHang");

            entity.HasOne(d => d.IdTourNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.IdTour)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhGia__IdTour__29221CFB");
        });

        modelBuilder.Entity<DanhMucTour>(entity =>
        {
            entity.HasKey(e => e.IdDanhMuc).HasName("PK__DanhMucT__7E5B713D00DFBC9C");

            entity.ToTable("DanhMucTour");

            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasMaxLength(100);
        });

        modelBuilder.Entity<DatTour>(entity =>
        {
            entity.HasKey(e => e.IdDatTour).HasName("PK__DatTour__BAA5E593F31555AD");

            entity.ToTable("DatTour");

            entity.Property(e => e.DiaChi).HasMaxLength(100);
            entity.Property(e => e.NgayDat).HasColumnType("datetime");
            entity.Property(e => e.PtthanhToan).HasColumnName("PTThanhToan");
            entity.Property(e => e.TenNguoiDat).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.IdGiamGiaNavigation).WithMany(p => p.DatTours)
                .HasForeignKey(d => d.IdGiamGia)
                .HasConstraintName("FK_DatTour_MaGiamGia");

            entity.HasOne(d => d.IdKhachHangNavigation).WithMany(p => p.DatTours)
                .HasForeignKey(d => d.IdKhachHang)
                .HasConstraintName("FK_DatTour_KhachHang");

            entity.HasOne(d => d.IdNhanVienNavigation).WithMany(p => p.DatTours)
                .HasForeignKey(d => d.IdNhanVien)
                .HasConstraintName("FK_DatTour_NhanVien");

            entity.HasOne(d => d.IdNkhNavigation).WithMany(p => p.DatTours)
                .HasForeignKey(d => d.IdNkh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatTour_NgayKhoiHanh");

            entity.HasOne(d => d.IdTourNavigation).WithMany(p => p.DatTours)
                .HasForeignKey(d => d.IdTour)
                .HasConstraintName("FK_DatTour_Tour");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.IdKhachHang).HasName("PK__KhachHan__7CF5D836A699AED6");

            entity.ToTable("KhachHang");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTenKh)
                .HasMaxLength(100)
                .HasColumnName("HoTenKH");
            entity.Property(e => e.MatKhau).HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<LienHe>(entity =>
        {
            entity.HasKey(e => e.IdLienHe).HasName("PK__LienHe__03AC912A3941DB9F");

            entity.ToTable("LienHe");

            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);

            entity.HasOne(d => d.IdKhachHangNavigation).WithMany(p => p.LienHes)
                .HasForeignKey(d => d.IdKhachHang)
                .HasConstraintName("FK__LienHe__IdKhachH__0880433F");
        });

        modelBuilder.Entity<MaGiamGium>(entity =>
        {
            entity.HasKey(e => e.IdGiamGia).HasName("PK__MaGiamGi__E0F7D8B6EBCFD7E8");

            entity.HasIndex(e => e.MaCode, "UQ__MaGiamGi__152C7C5C8F846D89").IsUnique();

            entity.Property(e => e.MaCode).HasMaxLength(50);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(100);
        });

        modelBuilder.Entity<NgayKhoiHanh>(entity =>
        {
            entity.HasKey(e => e.IdNkh).HasName("PK__NgayKhoi__0DD2F644DB2DC303");

            entity.ToTable("NgayKhoiHanh");

            entity.Property(e => e.IdNkh).HasColumnName("IdNKH");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.NgayKhoiHanh1)
                .HasColumnType("datetime")
                .HasColumnName("NgayKhoiHanh");
            entity.Property(e => e.SoChoConLai).HasDefaultValue(0);

            entity.HasOne(d => d.IdTourNavigation).WithMany(p => p.NgayKhoiHanhs)
                .HasForeignKey(d => d.IdTour)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NgayKhoiH__IdTou__634EBE90");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.IdNhanVien).HasName("PK__NhanVien__B8294845193F4AD8");

            entity.ToTable("NhanVien");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTenNv)
                .HasMaxLength(100)
                .HasColumnName("HoTenNV");
            entity.Property(e => e.MatKhau).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.IdTour).HasName("PK__Tour__860C736F4B546D4F");

            entity.ToTable("Tour");

            entity.Property(e => e.DiemDen).HasMaxLength(100);
            entity.Property(e => e.DiemKhoiHanh).HasMaxLength(100);
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.TenTour).HasMaxLength(255);
            entity.Property(e => e.ThoiGian).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasMaxLength(100);

            entity.HasOne(d => d.IdDanhMucNavigation).WithMany(p => p.Tours)
                .HasForeignKey(d => d.IdDanhMuc)
                .HasConstraintName("IdDanhMuc");
        });

        modelBuilder.Entity<TourHinhAnh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TourHinh__3214EC2777D68B66");

            entity.ToTable("TourHinhAnh");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.UrlHinhAnh).HasMaxLength(255);

            entity.HasOne(d => d.IdTourNavigation).WithMany(p => p.TourHinhAnhs)
                .HasForeignKey(d => d.IdTour)
                .HasConstraintName("FK__TourHinhA__IdTou__403A8C7D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
