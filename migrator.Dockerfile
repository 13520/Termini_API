FROM mcr.microsoft.com/dotnet/sdk:9.0

# Install dotnet-ef global tool and make sure it's on PATH
RUN dotnet tool install --global dotnet-ef --version 9.* \
    && export PATH="$PATH:/root/.dotnet/tools"

ENV PATH="/root/.dotnet/tools:${PATH}"
WORKDIR /src

# Copy only what is needed for restore to leverage Docker cache
COPY Termini_Api/Termini_Api.csproj Termini_Api/
RUN dotnet restore Termini_Api/Termini_Api.csproj

# Copy the rest of the repo
COPY . /src

# Run the migration (retry loop) when the container starts
ENTRYPOINT ["/bin/sh", "-c", "until dotnet ef database update --project Termini_Api/Termini_Api.csproj --startup-project Termini_Api/Termini_Api.csproj; do echo 'Migration failed, retrying in 3s...'; sleep 3; done"]