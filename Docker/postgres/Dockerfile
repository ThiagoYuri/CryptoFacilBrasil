FROM postgres:latest

# Instalar pacotes de locais e gerar a localidade pt_BR.UTF-8
RUN apt-get update && apt-get install -y locales \
    && sed -i '/pt_BR.UTF-8/s/^# //g' /etc/locale.gen \
    && locale-gen pt_BR.UTF-8 \
    && update-locale LANG=pt_BR.UTF-8

# Definir variáveis de ambiente para a localidade
ENV LANG pt_BR.UTF-8
ENV LANGUAGE pt_BR:pt
ENV LC_ALL pt_BR.UTF-8
