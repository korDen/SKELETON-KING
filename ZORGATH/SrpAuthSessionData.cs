﻿using SecureRemotePassword;
using System.Security.Cryptography;

namespace ZORGATH;

/// <summary>
///     A helper class for maintaining the state of an SRP authorisation.
/// </summary>
public class SrpAuthSessionData
{
    // The following constants match the HoN constants. These are the N and g for SRP.
    // Values courtesy of https://github.com/theli-ua/pyHoNBot/blob/master/hon/masterserver.py#L23-L24

    public const string N = "DA950C6C97918CAE89E4F5ECB32461032A217D740064BC12FC0723CD204BD02A7AE29B53F3310C13BA998B7910F8B6A14112CBC67BDD2427EDF494CB8BCA68510C0AAEE5346BD320845981546873069B337C073B9A9369D500873D647D261CCED571826E54C6089E7D5085DC2AF01FD861AE44C8E64BCA3EA4DCE942C5F5B89E5496C2741A9E7E9F509C261D104D11DD4494577038B33016E28D118AE4FD2E85D9C3557A2346FAECED3EDBE0F4D694411686BA6E65FEE43A772DC84D394ADAE5A14AF33817351D29DE074740AA263187AB18E3A25665EACAA8267C16CDE064B1D5AF0588893C89C1556D6AEF644A3BA6BA3F7DEC2F3D6FDC30AE43FBD6D144BB";

    public const string g = "2";

    public SrpAuthSessionData(string loginName, string clientPublicEphemeral, string salt, string passwordSalt, string hashedPassword)
    {
        ClientPublicEphemeral = clientPublicEphemeral;
        Salt = salt;
        PasswordSalt = passwordSalt;

        SrpParameters parameters = SrpParameters.Create<SHA256>(N, g);
        SrpClient srpClient = new(parameters);
        string privateClientKey = srpClient.DerivePrivateKey(salt, loginName, hashedPassword);
        Verifier = srpClient.DeriveVerifier(privateClientKey);

        SrpServer srpServer = new(parameters);
        ServerEphemeral = srpServer.GenerateEphemeral(Verifier);
    }

    public SrpAuthSessionData(string clientPublicEphemeral, string salt, string passwordSalt, string verifier, SrpEphemeral serverEphemeral)
    {
        ClientPublicEphemeral = clientPublicEphemeral;
        Salt = salt;
        PasswordSalt = passwordSalt;
        Verifier = verifier;
        ServerEphemeral = serverEphemeral;
    }

    /**
     * The public ephemeral `A` value sent from the client.
     */
    public readonly string ClientPublicEphemeral;

    /**
     * The public and secret ephemeral values generated by the server.
     */
    public readonly SrpEphemeral ServerEphemeral;

    /**
     * The salt (`s`), retrieved from the database using `LoginName` (`I`). The value is
     * generated randomly during registration and sent by the client.
     */
    public readonly string Salt;

    /**
     * The HoN `salt2`, retrieved from the database using `LoginName` (`I`). The value is
     * generated randomly during registration and sent by the client.
     */
    public readonly string PasswordSalt;

    public readonly string Verifier;
}
