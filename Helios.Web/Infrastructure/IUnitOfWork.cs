using System;

namespace Helios.Web.Infrastructure {
    public interface IUnitOfWork : IDisposable {
        int Commit();
    }
}