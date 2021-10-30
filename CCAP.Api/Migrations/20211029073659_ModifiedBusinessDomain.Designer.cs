﻿// <auto-generated />
using System;
using CCAP.Api.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CCAP.Api.Migrations
{
    [DbContext(typeof(CCAPContext))]
    [Migration("20211029073659_ModifiedBusinessDomain")]
    partial class ModifiedBusinessDomain
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("CCAP.Api.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AddressLine1")
                        .HasColumnType("text");

                    b.Property<string>("AddressLine2")
                        .HasColumnType("text");

                    b.Property<string>("AddressLine3")
                        .HasColumnType("text");

                    b.Property<int>("AppUserId")
                        .HasColumnType("integer");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<bool>("IsPermanent")
                        .HasColumnType("boolean");

                    b.Property<int>("PIN")
                        .HasColumnType("integer");

                    b.Property<string>("State")
                        .HasColumnType("text");

                    b.Property<bool>("UseForCommunication")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("CCAP.Api.Models.AppRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("RoleName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AppRoles");
                });

            modelBuilder.Entity("CCAP.Api.Models.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<int?>("Gender")
                        .HasColumnType("integer");

                    b.Property<bool>("IsForcedToResetPassword")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsStaff")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("MiddleName")
                        .HasColumnType("text");

                    b.Property<int>("NWrongAttempts")
                        .HasColumnType("integer");

                    b.Property<string>("PAN")
                        .HasColumnType("text");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("bytea");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("bytea");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("Qualification")
                        .HasColumnType("text");

                    b.Property<string>("SecondaryPhoneNumber")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AppUsers");
                });

            modelBuilder.Entity("CCAP.Api.Models.AppUserRole", b =>
                {
                    b.Property<int>("AppUserId")
                        .HasColumnType("integer");

                    b.Property<int>("AppRoleId")
                        .HasColumnType("integer");

                    b.HasKey("AppUserId", "AppRoleId");

                    b.HasIndex("AppRoleId");

                    b.ToTable("AppUserRoles");
                });

            modelBuilder.Entity("CCAP.Api.Models.ApplicationStatus", b =>
                {
                    b.Property<int>("AppUserId")
                        .HasColumnType("integer");

                    b.Property<int>("CreditCardApplicationId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateOfProcessing")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Remarks")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("AppUserId", "CreditCardApplicationId");

                    b.HasIndex("CreditCardApplicationId");

                    b.ToTable("ApplicationStatusList");
                });

            modelBuilder.Entity("CCAP.Api.Models.BankAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("AccountNumber")
                        .HasColumnType("bigint");

                    b.Property<int>("AppUserId")
                        .HasColumnType("integer");

                    b.Property<double>("Balance")
                        .HasColumnType("double precision");

                    b.Property<string>("Branch")
                        .HasColumnType("text");

                    b.Property<string>("BranchCode")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.ToTable("BankAccounts");
                });

            modelBuilder.Entity("CCAP.Api.Models.CreditCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("CardCode")
                        .HasColumnType("text");

                    b.Property<string>("Category")
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<int>("MaximumLimit")
                        .HasColumnType("integer");

                    b.Property<int>("MinimumAnnualIncome")
                        .HasColumnType("integer");

                    b.Property<int>("MinimumCreditScore")
                        .HasColumnType("integer");

                    b.Property<int>("MinimumLimit")
                        .HasColumnType("integer");

                    b.Property<string>("SubType")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("CreditCards");
                });

            modelBuilder.Entity("CCAP.Api.Models.CreditCardApplication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AnnualIncome")
                        .HasColumnType("integer");

                    b.Property<int>("CreditCardId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateOfApplication")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("EmploymentStatus")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreditCardId");

                    b.ToTable("CreditCardApplications");
                });

            modelBuilder.Entity("CCAP.Api.Models.Address", b =>
                {
                    b.HasOne("CCAP.Api.Models.AppUser", "AppUser")
                        .WithMany("Addresses")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("CCAP.Api.Models.AppUserRole", b =>
                {
                    b.HasOne("CCAP.Api.Models.AppRole", "AppRole")
                        .WithMany("AppUserRoles")
                        .HasForeignKey("AppRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CCAP.Api.Models.AppUser", "AppUser")
                        .WithMany("AppUserRoles")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppRole");

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("CCAP.Api.Models.ApplicationStatus", b =>
                {
                    b.HasOne("CCAP.Api.Models.AppUser", "AppUser")
                        .WithMany("StatusList")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CCAP.Api.Models.CreditCardApplication", "CreditCardApplication")
                        .WithMany("StatusList")
                        .HasForeignKey("CreditCardApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUser");

                    b.Navigation("CreditCardApplication");
                });

            modelBuilder.Entity("CCAP.Api.Models.BankAccount", b =>
                {
                    b.HasOne("CCAP.Api.Models.AppUser", "AppUser")
                        .WithMany("BankAccounts")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("CCAP.Api.Models.CreditCardApplication", b =>
                {
                    b.HasOne("CCAP.Api.Models.CreditCard", "CreditCard")
                        .WithMany("Applications")
                        .HasForeignKey("CreditCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreditCard");
                });

            modelBuilder.Entity("CCAP.Api.Models.AppRole", b =>
                {
                    b.Navigation("AppUserRoles");
                });

            modelBuilder.Entity("CCAP.Api.Models.AppUser", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("AppUserRoles");

                    b.Navigation("BankAccounts");

                    b.Navigation("StatusList");
                });

            modelBuilder.Entity("CCAP.Api.Models.CreditCard", b =>
                {
                    b.Navigation("Applications");
                });

            modelBuilder.Entity("CCAP.Api.Models.CreditCardApplication", b =>
                {
                    b.Navigation("StatusList");
                });
#pragma warning restore 612, 618
        }
    }
}